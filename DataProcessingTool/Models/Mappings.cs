using System;
using System.Text.RegularExpressions;

using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace DataProcessingTool.Models
{
    public class RoleAssignmentMapping : CsvMapping<RoleAssignment>
    {
        public RoleAssignmentMapping()
        {
            MapProperty(0, x => x.Subscription, new RoleAssignmentSubscriptionInfoConverter());
            MapProperty(1, x => x.SourceObjectId);
            MapProperty(2, x => x.SourceObjectName);
            MapProperty(3, x => x.SourceObjectType);
            MapProperty(4, x => x.TargetObjectId);
            MapProperty(5, x => x.TargetObjectName);
            MapProperty(6, x => x.RoleDefinitionName);
            MapProperty(7, x => x.RoleDefinitionId);
            MapProperty(8, x => x.CustomRole);
            MapProperty(9, x => x.Skip);
            MapProperty(10, x => x.Status);
            MapProperty(11, x => x.PathToSourceFile);
        }
    }

    public class KeyVaultUserMapping : CsvMapping<KeyVaultUser>
    {
        public KeyVaultUserMapping()
        {
            MapProperty(0, x => x.Vault);
            MapProperty(1, x => x.SourceObjId);
            MapProperty(2, x => x.User);
            MapProperty(3, x => x.Type);
            MapProperty(4, x => x.TargetObjId);
            MapProperty(5, x => x.Keys);
            MapProperty(6, x => x.Secrets);
            MapProperty(7, x => x.Certificates);
            MapProperty(8, x => x.AppId);
            MapProperty(9, x => x.Skip);
            MapProperty(10, x => x.Note);
            MapProperty(11, x => x.SubscriptionId);
            MapProperty(12, x => x.PathToSourceFile);
        }
    }

    public class CustomRoleMapping : CsvMapping<CustomRole>
    {
        public CustomRoleMapping()
        {
            MapProperty(0, x => x.Name);
            MapProperty(1, x => x.Id);
            MapProperty(2, x => x.IsCustom);
            MapProperty(3, x => x.Description);
            MapProperty(4, x => x.Actions);
            MapProperty(5, x => x.NotActions);
            MapProperty(6, x => x.DataActions);
            MapProperty(7, x => x.NotDataActions);
            MapProperty(8, x => x.AssignableScopes);
            MapProperty(9, x => x.PathToSourceFile);
        }
    }

    public class HardSourceMapping : CsvMapping<HardSource>
    {
        public HardSourceMapping()
        {
            MapProperty(0, x => x.ResourceName);
            MapProperty(1, x => x.ResourceType);
            MapProperty(2, x => x.Notes);
            MapProperty(3, x => x.MoreInfo);
            MapProperty(4, x => x.PathToSourceFile);
        }
    }

    public class RoleAssignmentSubscriptionInfoConverter : ITypeConverter<RoleAssignment.SubscriptionInfo>
    {
        private static readonly Regex IdRegex = new Regex(@"/subscriptions/([\w\d-]+).*", RegexOptions.Compiled);

        public Type TargetType => typeof(RoleAssignment.SubscriptionInfo);

        public bool TryConvert(string value, out RoleAssignment.SubscriptionInfo result)
        {
            string id = null;
            if (IdRegex.IsMatch(value))
            {
                id = IdRegex.Match(value).Groups[1].Value;
            }

            result = new RoleAssignment.SubscriptionInfo { Id = id, AccessOf = value };
            return true;
        }
    }
}