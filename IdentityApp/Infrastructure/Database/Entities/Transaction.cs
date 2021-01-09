using IdentityApp.Infrastructure.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Infrastructure.Database.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        [Required]
        public TransactionStatus Status { get; set; }
        [Required]
        public TransactionType Type { get; set; }
        [Required]
        public decimal Amount { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
