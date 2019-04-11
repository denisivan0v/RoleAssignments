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
            MapProperty(0, x => x.SubscriptionId);
            MapProperty(1, x => x.AccessOf);
            MapProperty(2, x => x.SourceObjectId);
            MapProperty(3, x => x.SourceObjectName);
            MapProperty(4, x => x.SourceObjectType);
            MapProperty(5, x => x.TargetObjectId);
            MapProperty(6, x => x.TargetObjectName);
            MapProperty(7, x => x.RoleDefinitionName);
            MapProperty(8, x => x.RoleDefinitionId);
            MapProperty(9, x => x.CustomRole);
            MapProperty(10, x => x.Skip);
            MapProperty(11, x => x.Status);
            MapProperty(12, x => x.PathToSourceFile);
        }
    }

    public class KeyVaultUserMapping : CsvMapping<KeyVaultUser>
    {
        public KeyVaultUserMapping()
        {
            MapProperty(0, x => x.SubscriptionId);
            MapProperty(1, x => x.Vault);
            MapProperty(2, x => x.SourceObjId);
            MapProperty(3, x => x.User);
            MapProperty(4, x => x.Type);
            MapProperty(5, x => x.TargetObjId);
            MapProperty(6, x => x.Keys);
            MapProperty(7, x => x.Secrets);
            MapProperty(8, x => x.Certificates);
            MapProperty(9, x => x.AppId);
            MapProperty(10, x => x.Skip);
            MapProperty(11, x => x.Note);
            MapProperty(12, x => x.RawSubscriptionId);
            MapProperty(13, x => x.PathToSourceFile);
        }
    }

    public class CustomRoleMapping : CsvMapping<CustomRole>
    {
        public CustomRoleMapping()
        {
            MapProperty(0, x => x.SubscriptionId);
            MapProperty(1, x => x.Name);
            MapProperty(2, x => x.Id);
            MapProperty(3, x => x.IsCustom);
            MapProperty(4, x => x.Description);
            MapProperty(5, x => x.Actions);
            MapProperty(6, x => x.NotActions);
            MapProperty(7, x => x.DataActions);
            MapProperty(8, x => x.NotDataActions);
            MapProperty(9, x => x.AssignableScopes);
            MapProperty(10, x => x.PathToSourceFile);
        }
    }

    public class HardResourceMapping : CsvMapping<HardResource>
    {
        public HardResourceMapping()
        {
            MapProperty(0, x => x.SubscriptionId);
            MapProperty(1, x => x.ResourceName);
            MapProperty(2, x => x.ResourceType);
            MapProperty(3, x => x.Notes);
            MapProperty(4, x => x.MoreInfo);
            MapProperty(5, x => x.PathToSourceFile);
        }
    }
}