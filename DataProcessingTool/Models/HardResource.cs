using System;

namespace DataProcessingTool.Models
{
    public class HardResource : IEquatable<HardResource>
    {
        public string SubscriptionId { get; set; }
        public string ResourceName { get; set; }
        public string ResourceType { get; set; }
        public string Notes { get; set; }
        public string MoreInfo { get; set; }
        public string PathToSourceFile { get; set; }

        public override bool Equals(object obj) => obj is HardResource other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(SubscriptionId, ResourceName, ResourceType);

        public bool Equals(HardResource other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return SubscriptionId == other.SubscriptionId &&
                   ResourceName == other.ResourceName &&
                   ResourceType == other.ResourceType;
        }
    }
}