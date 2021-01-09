using System.Runtime.Serialization;

namespace IdentityApp.Infrastructure.Database.Enums
{
    public enum TransactionStatus
    {
        [EnumMember(Value = "pending")]
        Pending,
        [EnumMember(Value = "completed")]
        Completed,
        [EnumMember(Value = "cancelled")]
        Cancelled
    }
}
