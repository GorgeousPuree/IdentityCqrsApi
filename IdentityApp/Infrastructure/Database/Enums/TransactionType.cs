using System.Runtime.Serialization;

namespace IdentityApp.Infrastructure.Database.Enums
{
    public enum TransactionType
    {
        [EnumMember(Value = "refill")]
        Refill,
        [EnumMember(Value = "withdrawal")]
        Withdrawal,
    }
}
