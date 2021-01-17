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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityApp.CQRS.Queries
{
    public class TransactionPageQuery : IRequest<OperationDataResult<TransactionPageQueryResult>>
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber has to be greater than {1}")]
        [BindProperty(Name = "page_number")]
        public int PageNumber { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "NumberOfItemsPerPage has to be greater or equal to {1}")]
        [BindProperty(Name = "number_of_items_per_page")]
        public int NumberOfItemsPerPage { get; set; } = 10;

        [BindProperty(Name = "statuses")]
        public List<TransactionStatus> TransactionStatuses { get; set; } = new List<TransactionStatus>();

        [BindProperty(Name = "types")]
        public List<TransactionType> TransactionTypes { get; set; } = new List<TransactionType>();
    }

    public class TransactionPageQueryHandler : IRequestHandler<TransactionPageQuery,
        OperationDataResult<TransactionPageQueryResult>>
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public TransactionPageQueryHandler(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<OperationDataResult<TransactionPageQueryResult>> Handle(TransactionPageQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                var transactionsQuery = _applicationDbContext.Transactions.AsQueryable() // ambiguous invocation of System.Linq.Queryable and System.Linq.AsyncEnumerable
                    .OrderBy(transaction => transaction.Id)
                    .AsQueryable();

                if (query.TransactionStatuses.Count != 0)
                {
                    transactionsQuery = transactionsQuery.Where(transaction => query.TransactionStatuses.Contains(transaction.Status));
                }

                if (query.TransactionTypes.Count != 0)
                {
                    transactionsQuery = transactionsQuery.Where(transaction => query.TransactionTypes.Contains(transaction.Type));
                }

                var count = await transactionsQuery.Select(transaction => transaction.Id)
                    .CountAsync(cancellationToken);

                var transactions = await transactionsQuery
                    .Skip((query.PageNumber - 1) * query.NumberOfItemsPerPage)
                    .Take(query.NumberOfItemsPerPage)
                    .Select(transaction => new TransactionModel
                    {
                        Id = transaction.Id,
                        Status = transaction.Status,
                        Type = transaction.Type,
                        ClientName = transaction.Client.Name,
                        Amount = transaction.Amount
                    })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return new OperationDataResult<TransactionPageQueryResult>(true,
                    new TransactionPageQueryResult(count, transactions));
            }
            catch (Exception e)
            {
                return new OperationDataResult<TransactionPageQueryResult>(false, new List<string> { e.Message });
            }
        }
    }
}
