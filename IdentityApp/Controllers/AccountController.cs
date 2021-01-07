using IdentityApp.CQRS.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        /// <response code="200">Returns messages stating why the user could not be created.</response>
        /// <response code="201">Creates a user.</response>
        /// <response code="400">If invalid model is passed.</response>      
        /// <response code="500">If server error occurs.</response>      
        [HttpPost]
        [Route("api/account/registration")]
        [Produces("application/json")]
        public async Task<IActionResult> Register(CreateUserCommand createUserCommand)
        {
            var result = await _mediator.Send(createUserCommand);

            if (!result.Succeeded)
            {
                return StatusCode(500, result);
            }

            if (result.Messages[0] != "Succeeded")
            {
                return Ok(result);
            }

            return Created("api/account/registration", result);
        }

        /// <summary>
        /// Logins a user with given credentials.
        /// </summary>
        /// <response code="200">Successfully logins a user and returns HttpOnly cookie with jwt set in it.</response>
        /// <response code="400">If invalid model is passed.</response>      
        /// <response code="500">If server error occurs.</response>   
        [HttpPost]
        [Route("api/account/login")]
        [Produces("application/json")]
        public async Task<IActionResult> Login(LoginUserCommand loginUserCommand)
        {
            var result = await _mediator.Send(loginUserCommand);

            if (!result.Succeeded)
            {
                return StatusCode(500, result);
            }

            if (result.Messages[0] != "Succeeded")
            {
                return Ok(result);
            }

            HttpContext.Response.Cookies.Append(".AspNetCore.Application.Id", result.Model); // Nonsense name for security purposes. 

            return Ok();
        }
    }
}