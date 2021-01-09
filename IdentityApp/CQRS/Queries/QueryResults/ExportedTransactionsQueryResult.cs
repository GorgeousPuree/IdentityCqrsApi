namespace IdentityApp.CQRS.Queries.QueryResults
{
    public class ExportedTransactionsQueryResult
    {
        public byte[] FileData { get; set; }

        public ExportedTransactionsQueryResult(byte[] fileData)
        {
            FileData = fileData;
        }
    }
}
