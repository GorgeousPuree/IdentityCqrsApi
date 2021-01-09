using System.Collections.Generic;
using IdentityApp.Infrastructure.Models;

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
