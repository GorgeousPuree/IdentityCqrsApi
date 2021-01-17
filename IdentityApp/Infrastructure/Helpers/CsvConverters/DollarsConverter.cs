using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;

namespace IdentityApp.Infrastructure.Helpers.CsvConverters
{
    public class DollarsConverter : DefaultTypeConverter
    {
        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return string.Format($"${value:0.00}");
        }

        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            var newStr = text.Replace("$", String.Empty);
            //return base.ConvertFromString(text, row, memberMapData);
            return decimal.Parse(newStr);
        }
    }
}
