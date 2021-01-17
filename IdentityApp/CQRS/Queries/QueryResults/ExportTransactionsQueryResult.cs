namespace IdentityApp.CQRS.Queries.QueryResults
{
    public class ExportTransactionsQueryResult
    {
        public byte[] FileData { get; set; }

        public ExportTransactionsQueryResult(byte[] fileData)
        {
            FileData = fileData;
        }
    }
}
