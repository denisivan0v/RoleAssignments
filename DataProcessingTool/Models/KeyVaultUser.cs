using System;

namespace DataProcessingTool.Models
{
    public class KeyVaultUser : IEquatable<KeyVaultUser>
    {
        public string SubscriptionId { get; set; }
        public string Vault { get; set; }
        public string SourceObjId { get; set; }
        public string User { get; set; }
        public string Type { get; set; }
        public string TargetObjId { get; set; }
        public string Keys { get; set; }
        public string Secrets { get; set; }
        public string Certificates { get; set; }
        public string AppId { get; set; }
        public string Skip { get; set; }
        public string Note { get; set; }
        public string RawSubscriptionId { get; set; }
        public string PathToSourceFile { get; set; }

        public override bool Equals(object obj) => obj is KeyVaultUser other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(
            SubscriptionId,
            Vault,
            SourceObjId,
            User,
            Type,
            TargetObjId);

        public bool Equals(KeyVaultUser other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return SubscriptionId == other.SubscriptionId &&
                   Vault == other.Vault &&
                   SourceObjId == other.SourceObjId &&
                   User == other.User &&
                   Type == other.Type &&
                   TargetObjId == other.TargetObjId;
        }
    }
}