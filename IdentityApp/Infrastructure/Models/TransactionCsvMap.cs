using CsvHelper.Configuration;
using IdentityApp.Infrastructure.Database.Enums;
using IdentityApp.Infrastructure.Helpers.CsvConverters;
using System;

namespace IdentityApp.Infrastructure.Models
{
    public class TransactionCsvMap : ClassMap<TransactionModel>
    {
        public TransactionCsvMap()
        {
            Map(m => m.Id).Name("TransactionId");
            Map(m => m.Amount).TypeConverter<DollarsConverter>();
            Map(m => m.Status).Validate(field =>
            {
                Enum.TryParse(typeof(TransactionStatus), field, out _);
                return true;
            });
            Map(m => m.Type).Validate(field =>
            {
                Enum.TryParse(typeof(TransactionType), field, out _);
                return true;
            });
            Map(m => m.ClientName).Name("ClientName");
        }
    }
}
