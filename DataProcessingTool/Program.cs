using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CsvHelper;

using DataProcessingTool.Models;

using McMaster.Extensions.CommandLineUtils;

using TinyCsvParser;

namespace DataProcessingTool
{
    internal static class Program
    {
        private const int DegreeOfParallelism = 32;

        private const string AclsRequireFurtherReview = "ACLsRequireFurtherReview.csv";
        private const string ClassicRoleAssignments = "ClassicRoleAssignments.csv";
        private const string ObjectIdMapping = "ObjectIdMapping.csv";
        private const string CustomRoles = "customroles.csv";
        private const string AkvUserMapping = "AKVUserMapping.csv";
        private const string HardResources = "hardresources.csv";

        // \\rtcfile1\public\PATALV\Torus\CORP\7769d258-2b01-4fc4-8b4d-73d011108f3e\hardresources.csv
        // or /Volumes/CORP/0ff5a831-4084-4671-b2bc-41cece893e75/hardresources.csv
        private static readonly Regex SubscriptionIdRegex = new Regex(@".+[\\/]([\w\d-]+)[\\/]\w+\.csv$", RegexOptions.Compiled);

        private static int Main(string[] args)
        {
            var app = new CommandLineApplication
                {
                    Name = "DataProcessingTool",
                    Description = "Role assignments data processing tool",
                    ThrowOnUnexpectedArgument = false,
                    MakeSuggestionsInErrorMessage = true
                };
            app.HelpOption(inherited: true);

            var inputDirectories = app.Option(
                "-d|--dirs",
                "A list of directories that contain input data.",
                CommandOptionType.MultipleValue).IsRequired();

            app.OnExecute(() => OnExecuteAsync(inputDirectories.Values));

            return app.Execute(args);
        }

        private static async Task<int> OnExecuteAsync(IEnumerable<string> inputDirectories)
        {
            try
            {
                var data = await ReadDataAsync(inputDirectories);

                var merged = new Dictionary<string, string>();

                var filesToMerge = new[] { AclsRequireFurtherReview, ClassicRoleAssignments, ObjectIdMapping, CustomRoles, AkvUserMapping, HardResources };
                foreach (var fileName in filesToMerge)
                {
                    var files = data.Where(x => x.Key.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));
                    var contents = MergeFiles(files);

                    merged.Add(fileName, contents);

                    await File.WriteAllTextAsync(fileName, contents);
                }

                var readerOptions = new CsvReaderOptions(new[] { Environment.NewLine });
                var parserOptions = new CsvParserOptions(true, ',');

                var roleAssignments = new HashSet<RoleAssignment>();
                var roleAssignmentsParser = new CsvParser<RoleAssignment>(parserOptions, new RoleAssignmentMapping());
                roleAssignments.UnionWith(
                    roleAssignmentsParser.ReadFromString(readerOptions, merged[ClassicRoleAssignments] ?? string.Empty)
                                         .Where(x => x.IsValid)
                                         .Select(x => x.Result));

                roleAssignments.UnionWith(
                    roleAssignmentsParser.ReadFromString(readerOptions, merged[AclsRequireFurtherReview] ?? string.Empty)
                                         .Where(x => x.IsValid)
                                         .Select(x => x.Result));

                roleAssignments.UnionWith(
                    roleAssignmentsParser.ReadFromString(readerOptions, merged[ObjectIdMapping] ?? string.Empty)
                                         .Where(x => x.IsValid)
                                         .Select(x => x.Result));

                var userKeyVaults = new HashSet<KeyVaultUser>();
                var userKeyVaultParser = new CsvParser<KeyVaultUser>(parserOptions, new KeyVaultUserMapping());
                userKeyVaults.UnionWith(
                    userKeyVaultParser.ReadFromString(readerOptions, merged[AkvUserMapping] ?? string.Empty)
                                      .Where(x => x.IsValid)
                                      .Select(x => x.Result));

                var customRoles = new HashSet<CustomRole>();
                var customRoleParser = new CsvParser<CustomRole>(parserOptions, new CustomRoleMapping());
                customRoles.UnionWith(customRoleParser
                                  .ReadFromString(readerOptions, merged[CustomRoles] ?? string.Empty)
                                  .Where(x => x.IsValid)
                                  .Select(x => x.Result));

                var hardResources = new HashSet<HardResource>();
                var hardResourceParser = new CsvParser<HardResource>(parserOptions, new HardResourceMapping());
                hardResources.UnionWith(
                    hardResourceParser
                        .ReadFromString(readerOptions, merged[HardResources] ?? string.Empty)
                        .Where(x => x.IsValid)
                        .Select(x => x.Result));

                var query = from assignment in roleAssignments
                            join customRole in customRoles
                                on (assignment.SubscriptionId, assignment.RoleDefinitionId)
                                equals (customRole.SubscriptionId, customRole.Id)
                                into roles
                            from customRole in roles.DefaultIfEmpty()
                            join userKeyVault in userKeyVaults
                                on (assignment.SubscriptionId, assignment.SourceObjectId)
                                equals (userKeyVault.SubscriptionId, userKeyVault.SourceObjId)
                                into keyVaults
                            from keyVault in keyVaults.DefaultIfEmpty()
                            join hardResource in hardResources
                                on assignment.SubscriptionId equals hardResource.SubscriptionId
                                into resources
                            from hardResource in resources.DefaultIfEmpty()
                            select new
                                {
                                    assignment.SubscriptionId,
                                    assignment.AccessOf,
                                    assignment.SourceObjectId,
                                    SourceGroupName = assignment.SourceObjectName,
                                    assignment.SourceObjectType,
                                    assignment.TargetObjectId,
                                    TargetGroupName = assignment.TargetObjectName,
                                    assignment.Skip,
                                    assignment.Status,
                                    AssignmentPathToSourceFile = assignment.PathToSourceFile,
                                    assignment.RoleDefinitionName,
                                    assignment.RoleDefinitionId,
                                    assignment.CustomRole,
                                    CustomRoleId = customRole?.Id,
                                    CustomRoleName = customRole?.Name,
                                    customRole?.IsCustom,
                                    CustomRoleDescription = customRole?.Description,
                                    CustomRoleActions = customRole?.Actions,
                                    CustomRoleNotActions = customRole?.NotActions,
                                    CustomRoleDataActions = customRole?.DataActions,
                                    CustomRoleNotDataActions = customRole?.NotDataActions,
                                    CustomRoleAssignableScopes = customRole?.AssignableScopes,
                                    CustomRolePathToSourceFile = customRole?.PathToSourceFile,
                                    keyVault?.Vault,
                                    keyVault?.SourceObjId,
                                    keyVault?.User,
                                    keyVault?.Type,
                                    keyVault?.TargetObjId,
                                    keyVault?.Keys,
                                    keyVault?.Secrets,
                                    keyVault?.Certificates,
                                    keyVault?.AppId,
                                    KeyVaultSkip = keyVault?.Skip,
                                    KeyVaultNote = keyVault?.Note,
                                    KeyVaultSubscriptionId = keyVault?.RawSubscriptionId,
                                    KeyVaultPathToSourceFile = keyVault?.PathToSourceFile,
                                    hardResource?.ResourceName,
                                    hardResource?.ResourceType,
                                    HardResourceNotes = hardResource?.Notes,
                                    HardResourceMoreInfo = hardResource?.MoreInfo,
                                    HardResourcePathToSourceFile = hardResource?.PathToSourceFile
                                };

                var records = query.ToList();

                using (var writer = new StreamWriter("RoleAssignments.csv"))
                {
                    using (var csv = new CsvWriter(writer))
                    {
                        csv.WriteRecords(records);
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 2;
            }
        }

        private static async Task<IDictionary<string, string>> ReadDataAsync(IEnumerable<string> inputDirectories)
        {
            var result = new ConcurrentDictionary<string, string>();
            foreach (var directory in inputDirectories)
            {
                var directories = Directory.GetDirectories(directory)
                                           .Take(10)
                                           .ToList();
                Console.WriteLine($"The directory {directory} contains {directories.Count} subdirectories.");

                var partitions = Partitioner.Create(directories).GetOrderablePartitions(DegreeOfParallelism);

                var tasks = partitions.Select(
                    partition => Task.Run(
                        async () =>
                            {
                                while (partition.MoveNext())
                                {
                                    var path = partition.Current.Value;

                                    var files = Directory.GetFiles(path, "*.csv");
                                    foreach (var file in files)
                                    {
                                        var content = await File.ReadAllTextAsync(file);
                                        if (!result.TryAdd(file, content))
                                        {
                                            Console.WriteLine($"The file {file} could not be read.");
                                        }
                                    }

                                    Console.WriteLine($"{files.Length} files from the path {path} were read.");
                                }
                            }));
                await Task.WhenAll(tasks);
            }

            return result;
        }

        // ReSharper disable PossibleMultipleEnumeration
        private static string MergeFiles(IEnumerable<KeyValuePair<string, string>> files)
        {
            if (!files.Any())
            {
                return null;
            }

            var headers = ReadHeaders(files.First().Value);

            var merged = new StringBuilder();
            merged.AppendFormat("\"{0}\",{1},\"{2}\"{3}", "SubscriptionId", headers, "Path To Source File", Environment.NewLine);

            foreach (var (path, content) in files)
            {
                using (var reader = new StringReader(content))
                {
                    // Skipping the first two lines
                    if (reader.ReadLine() == null)
                    {
                        continue;
                    }

                    if (reader.ReadLine() == null)
                    {
                        continue;
                    }

                    string subscriptionId = null;
                    if (SubscriptionIdRegex.IsMatch(path))
                    {
                        subscriptionId = SubscriptionIdRegex.Match(path).Groups[1].Value;
                    }

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        merged.AppendFormat("\"{0}\",{1},\"{2}\"{3}", subscriptionId, line, path, Environment.NewLine);
                    }
                }
            }

            return merged.ToString();
        }

        private static string ReadHeaders(string fileContent)
        {
            using (var reader = new StringReader(fileContent))
            {
                // Skip first line
                if (reader.ReadLine() == null)
                {
                    return null;
                }

                return reader.ReadLine();
            }
        }
    }
}