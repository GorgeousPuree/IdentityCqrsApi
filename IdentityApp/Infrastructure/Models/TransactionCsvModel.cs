using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;
using IdentityApp.Infrastructure.Database.Enums;
using IdentityApp.Infrastructure.Helpers.CsvConverters;

namespace IdentityApp.Infrastructure.Models
{
    public class TransactionCsvModel
    {
        [Name("TransactionId")]
        public int Id { get; set; }
        public TransactionStatus Status { get; set; }
        public TransactionType Type { get; set; }
        public string ClientName { get; set; }

        [TypeConverter(typeof(DollarsConverter))]
        public decimal Amount { get; set; }
    }
}
