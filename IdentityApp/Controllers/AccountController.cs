using IdentityApp.CQRS.Commands.Account;
using IdentityApp.CQRS.Queries.Account;
using IdentityApp.Infrastructure.Helpers.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityApp.Controllers
{
    [ApiController]
    [Produces(typeof(OperationDataResult<bool>))]
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
        /// 'model' property shows whether the account was created. If false, 'messages' property will be filled with explanatory data.</response>
        /// <response code="201">Returns if service successfully handles the request and creates a user.</response>
        /// <response code="400">If invalid model is passed.</response>      
        /// <response code="500">If server error occurs.</response>      
        [HttpPost]
        [Route("api/account/registration")]
        [Produces("application/json")]
        public async Task<IActionResult> Register([FromBody] CreateUserCommand createUserCommand)
        {
            var result = await _mediator.Send(createUserCommand);

            if (!result.Succeeded)
            {
                return StatusCode(500, result);
            }

            if (!result.Model)
            {
                return Ok(result);
            }

            return Created("api/account/registration", result);
        }

        /// <summary>
        /// Logins a user with given credentials.
        /// </summary>
        /// <response code="200">Returns if service successfully handles the request.
        /// 'model' property shows whether the user has logged in or not.
        /// If 'model' equals to true, server returns HttpOnly cookie with jwt set in it.</response>
        /// <response code="400">If invalid model is passed.</response>      
        /// <response code="500">If server error occurs.</response>   
        [HttpPost]
        [Route("api/account/login")]
        [Produces("application/json")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand loginUserCommand)
        {
            var result = await _mediator.Send(loginUserCommand);

            if (!result.Succeeded)
            {
                return StatusCode(500, result);
            }

            if (!result.Model.AccountExists)
            {
                return Ok(new OperationDataResult<bool>(true, false));
            }

            HttpContext.Response.Cookies.Append(".AspNetCore.Application.Id", result.Model.Jwt); // Nonsense name for security purposes. 

            return Ok(new OperationDataResult<bool>(true, true));
        }

        /// <summary>
        /// Checks whether the username was already taken or not.
        /// </summary>
        /// <response code="200">Returns if service successfully handles the request.
        /// 'model' property shows whether the username was already taken or not.</response>
        /// <response code="400">If invalid model is passed.</response>      
        /// <response code="500">If server error occurs.</response>   
        [HttpGet]
        [Route("api/account/username-taken")]
        [Produces("application/json")]
        public async Task<IActionResult> IsUsernameTaken([FromQuery] IsUsernameTakenQuery isUsernameTakenQuery)
        {
            var result = await _mediator.Send(isUsernameTakenQuery);

            return !result.Succeeded ? StatusCode(500, result) : Ok(result);
        }
    }
}