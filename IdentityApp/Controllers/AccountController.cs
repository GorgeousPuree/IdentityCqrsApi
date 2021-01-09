using IdentityApp.Infrastructure.Helpers.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IdentityApp.CQRS.Commands;
using IdentityApp.CQRS.Commands.CommandResults;
using IdentityApp.CQRS.Queries;
using IdentityApp.CQRS.Queries.QueryResults;

namespace IdentityApp.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a user with given credentials.
        /// </summary>
        /// <response code="200">Returns if service successfully handles the request.
        /// 'model.IsCreated' property shows whether the account was created. If false, 'messages' property will be filled with explanatory data.</response>
        /// <response code="201">Returns if service successfully handles the request and creates a user.</response>
        /// <response code="400">If invalid model is passed.</response>      
        /// <response code="500">If server error occurs.</response>      
        [HttpPost]
        [Route("api/account/registration")]
        [Produces(typeof(OperationDataResult<CreateUserCommandResult>))]
        public async Task<IActionResult> Register([FromBody] CreateUserCommand createUserCommand)
        {
            var result = await _mediator.Send(createUserCommand);

            if (!result.Succeeded)
            {
                return StatusCode(500, result);
            }

            if (!result.Model.IsCreated)
            {
                return Ok(result);
            }

            return Created("api/account/registration", result);
        }

        /// <summary>
        /// Logins a user with given credentials.
        /// </summary>
        /// <response code="200">Returns if service successfully handles the request.
        /// If 'model.LoggedIn' equals to true, server returns HttpOnly cookie with jwt set in it.</response>
        /// <response code="400">If invalid model is passed.</response>      
        /// <response code="500">If server error occurs.</response>   
        [HttpPost]
        [Route("api/account/login")]
        [Produces(typeof(OperationDataResult<LoginUserCommandResult>))]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand loginUserCommand)
        {
            var (result, jwt) = await _mediator.Send(loginUserCommand);

            if (!result.Succeeded)
            {
                return StatusCode(500, result);
            }

            if (!result.Model.LoggedIn)
            {
                return Ok(result);
            }

            HttpContext.Response.Cookies.Append(".AspNetCore.Application.Id", jwt); // Nonsense name for security purposes. 

            return Ok(result);
        }

        /// <summary>
        /// Checks whether the username was already taken or not.
        /// </summary>
        /// <response code="200">Returns if service successfully handles the request.
        /// 'model.IsTaken' property shows whether the username was already taken or not.</response>
        /// <response code="400">If invalid model is passed.</response>      
        /// <response code="500">If server error occurs.</response>   
        [HttpGet]
        [Route("api/account/username-taken")]
        [Produces(typeof(OperationDataResult<IsUsernameTakenQueryResult>))]
        public async Task<IActionResult> IsUsernameTaken([FromQuery] IsUsernameTakenQuery isUsernameTakenQuery)
        {
            var result = await _mediator.Send(isUsernameTakenQuery);

            return !result.Succeeded ? StatusCode(500, result) : Ok(result);
        }
    }
}