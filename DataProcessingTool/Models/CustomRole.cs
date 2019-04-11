using System;

namespace DataProcessingTool.Models
{
    public class CustomRole : IEquatable<CustomRole>
    {
        public string SubscriptionId { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string IsCustom { get; set; }
        public string Description { get; set; }
        public string Actions { get; set; }
        public string NotActions { get; set; }
        public string DataActions { get; set; }
        public string NotDataActions { get; set; }
        public string AssignableScopes { get; set; }
        public string PathToSourceFile { get; set; }

        public override bool Equals(object obj) => obj is CustomRole other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(SubscriptionId, Name, Id, IsCustom);

        public bool Equals(CustomRole other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return SubscriptionId == other.SubscriptionId &&
                   Name == other.Name &&
                   Id == other.Id &&
                   IsCustom == other.IsCustom;
        }
    }
}