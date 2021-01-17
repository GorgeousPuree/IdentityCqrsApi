using CsvHelper;
using IdentityApp.CQRS.Queries.QueryResults;
using IdentityApp.Infrastructure.Database;
using IdentityApp.Infrastructure.Database.Enums;
using IdentityApp.Infrastructure.Helpers.Responses;
using IdentityApp.Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityApp.CQRS.Queries
{
    public class ExportTransactionsQuery : IRequest<OperationDataResult<ExportTransactionsQueryResult>>
    {
        [BindProperty(Name = "statuses")]
        public List<TransactionStatus> TransactionStatuses { get; set; } = new List<TransactionStatus>();

        [BindProperty(Name = "types")]
        public List<TransactionType> TransactionTypes { get; set; } = new List<TransactionType>();
    }

    public class ExportTransactionsQueryHandler : IRequestHandler<ExportTransactionsQuery,
        OperationDataResult<ExportTransactionsQueryResult>>
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ExportTransactionsQueryHandler(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<OperationDataResult<ExportTransactionsQueryResult>> Handle(ExportTransactionsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var transactionsQuery = _applicationDbContext.Transactions.AsQueryable() // ambiguous invocation of System.Linq.Queryable and System.Linq.AsyncEnumerable 
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
                    .Select(transaction => new TransactionModel
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
                    csv.Configuration.Delimiter = ",";
                    csv.Configuration.RegisterClassMap<TransactionCsvMap>();

                    csv.WriteHeader<TransactionModel>();
                    await csv.NextRecordAsync();
                    await csv.WriteRecordsAsync(transactions);
                }
                return new OperationDataResult<ExportTransactionsQueryResult>(true, new ExportTransactionsQueryResult(ms.ToArray()));
            }
            catch (Exception e)
            {
                return new OperationDataResult<ExportTransactionsQueryResult>(false, new List<string> { e.Message });
            }
        }
    }
}
