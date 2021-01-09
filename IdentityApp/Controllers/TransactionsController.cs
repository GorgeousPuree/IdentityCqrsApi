using IdentityApp.CQRS.Queries;
using IdentityApp.CQRS.Queries.QueryResults;
using IdentityApp.Infrastructure.Helpers.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityApp.Controllers
{
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Returns a list of transactions (transaction page) relying on given parameters.
        /// </summary>
        /// <response code="200">Returns if service successfully handles the request.
        /// 'model' property contains wanted transactions list and total count of transactions relying on the given filters (statuses and types).</response>
        /// <response code="500">If server error occurs.</response>  
        [HttpGet]
        [Route("api/transactions")]
        [Produces(typeof(OperationDataResult<TransactionPageQueryResult>))]
        public async Task<IActionResult> GetTransactionsPage([FromQuery] TransactionPageQuery transactionPageQuery)
        {
            var result = await _mediator.Send(transactionPageQuery);

            return !result.Succeeded ? StatusCode(500, result) : Ok(result);
        }

        /// <summary>
        /// Exports a transactions csv based on the given filters (statuses and types).
        /// </summary>
        /// <response code="200">Returns if service successfully handles the request.
        /// Returns a csv file.</response>
        /// <response code="500">If server error occurs.</response>  
        [HttpGet]
        [Route("api/transactions/export")]
        public async Task<IActionResult> GetExportedTransactions([FromQuery] ExportedTransactionsQuery transactionPageQuery)
        {
            var result = await _mediator.Send(transactionPageQuery);

            if (!result.Succeeded)
            {
                return StatusCode(500, result);
            }

            return File(result.Model.FileData, "text/csv", "data.csv");
        }
    }
}