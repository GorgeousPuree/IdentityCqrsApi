namespace IdentityApp.CQRS.Commands.CommandResults
{
    public class ImportTransactionsCommandResult
    {
        public bool Imported { get; set; }

        public ImportTransactionsCommandResult(bool imported)
        {
            Imported = imported;
        }
    }
}
