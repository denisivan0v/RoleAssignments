using System;

namespace DataProcessingTool.Models
{
    public class HardSource : IEquatable<HardSource>
    {
        public string ResourceName { get; set; }
        public string ResourceType { get; set; }
        public string Notes { get; set; }
        public string MoreInfo { get; set; }
        public string PathToSourceFile { get; set; }

        public override bool Equals(object obj) => obj is HardSource other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(ResourceName, ResourceType);

        public bool Equals(HardSource other)
        {
            return ResourceName == other.ResourceName &&
                   ResourceType == other.ResourceType;
        }
    }
}