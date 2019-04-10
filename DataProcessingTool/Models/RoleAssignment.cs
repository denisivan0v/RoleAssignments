using System;

namespace DataProcessingTool.Models
{
    public class RoleAssignment : IEquatable<RoleAssignment>
    {
        public SubscriptionInfo Subscription { get; set; }
        public string SourceObjectId { get; set; }
        public string SourceObjectName { get; set; }
        public string SourceObjectType { get; set; }
        public string TargetObjectId { get; set; }
        public string TargetObjectName { get; set; }
        public string RoleDefinitionName { get; set; }
        public string RoleDefinitionId { get; set; }
        public string CustomRole { get; set; }
        public string Skip { get; set; }
        public string Status { get; set; }
        public string PathToSourceFile { get; set; }

        public override bool Equals(object obj) => obj is RoleAssignment other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(
                Subscription.Id,
                SourceObjectId,
                SourceObjectType,
                TargetObjectId,
                RoleDefinitionId);

        public bool Equals(RoleAssignment other)
        {
            return Subscription.Id == other.Subscription.Id &&
                   SourceObjectId == other.SourceObjectId &&
                   SourceObjectType == other.SourceObjectType &&
                   TargetObjectId == other.TargetObjectId &&
                   RoleDefinitionId == other.RoleDefinitionId;
        }

        public class SubscriptionInfo
        {
            public string Id { get; set; }
            public string AccessOf { get; set; }
        }
    }
}