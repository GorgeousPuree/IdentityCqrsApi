using IdentityApp.Infrastructure.Models;
using System.Collections.Generic;

namespace IdentityApp.CQRS.Queries.QueryResults
{
    public class TransactionPageQueryResult
    {
        public List<TransactionModel> Transactions { get; set; }
        public int TotalTransactionsCount { get; set; }

        public TransactionPageQueryResult(int totalTransactionsCount, List<TransactionModel> transactions)
        {
            TotalTransactionsCount = totalTransactionsCount;
            Transactions = transactions;
        }
    }
}
