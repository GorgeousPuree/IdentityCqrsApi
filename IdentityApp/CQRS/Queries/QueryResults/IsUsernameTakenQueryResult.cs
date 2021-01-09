namespace IdentityApp.CQRS.Queries.QueryResults
{
    public class IsUsernameTakenQueryResult
    {
        public bool IsTaken { get; set; }

        public IsUsernameTakenQueryResult(bool isTaken)
        {
            IsTaken = isTaken;
        }
    }
}
