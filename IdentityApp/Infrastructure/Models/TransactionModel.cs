using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Infrastructure.Database.Enums;

namespace IdentityApp.Infrastructure.Models
{
    public class TransactionModel
    {
        public int Id { get; set; }
        public TransactionStatus Status { get; set; }
        public TransactionType Type { get; set; }
        public string ClientName { get; set; }
        public decimal Amount { get; set; }
    }
}
