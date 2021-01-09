using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using IdentityApp.CQRS.Queries.QueryResults;
using IdentityApp.Infrastructure.Database;
using IdentityApp.Infrastructure.Database.Enums;
using IdentityApp.Infrastructure.Helpers.Responses;
using IdentityApp.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.CQRS.Queries
{
    public class ExportedTransactionsQuery : IRequest<OperationDataResult<ExportedTransactionsQueryResult>>
    {
        public List<TransactionStatus> TransactionStatuses { get; set; } = new List<TransactionStatus>();
        public List<TransactionType> TransactionTypes { get; set; } = new List<TransactionType>();
    }

    public class ExportedTransactionsQueryHandler : IRequestHandler<ExportedTransactionsQuery,
        OperationDataResult<ExportedTransactionsQueryResult>>
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ExportedTransactionsQueryHandler(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<OperationDataResult<ExportedTransactionsQueryResult>> Handle(ExportedTransactionsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var transactionsQuery = _applicationDbContext.Transactions
                    .OrderBy(transaction => transaction.Id)
                    .AsQueryable();

                if (query.TransactionStatuses.Count != 0)
                {
                    transactionsQuery = transactionsQuery
                        .Where(transaction => query.TransactionStatuses
                            .Contains(transaction.Status));
                }

                if (query.TransactionTypes.Count != 0)
                {
                    transactionsQuery = transactionsQuery
                        .Where(transaction => query.TransactionTypes
                            .Contains(transaction.Type));
                }

                var transactions = await transactionsQuery
                    .Select(transaction => new TransactionCsvModel
                    {
                        Id = transaction.Id,
                        Amount = transaction.Amount,
                        ClientName = transaction.Client.Name,
                        Status = transaction.Status,
                        Type = transaction.Type
                    })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                // TODO: refactor
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

                await using var ms = new MemoryStream();
                await using var writer = new StreamWriter(ms);
                await using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
                {
                    var classMap = csv.Configuration.AutoMap<TransactionCsvModel>();
                    csv.Configuration.RegisterClassMap(classMap);
                    csv.Configuration.Delimiter = ",";

                    csv.WriteHeader<TransactionCsvModel>();
                    await csv.NextRecordAsync();
                    await csv.WriteRecordsAsync(transactions);
                }
                return new OperationDataResult<ExportedTransactionsQueryResult>(true, new ExportedTransactionsQueryResult(ms.ToArray()));
            }
            catch (Exception e)
            {
                return new OperationDataResult<ExportedTransactionsQueryResult>(false, new List<string> { e.Message });
            }
        }
    }
}
