using System;
using IdentityApp.Infrastructure.CQRS.Commands;
using IdentityApp.Infrastructure.Helpers.Auth;
using IdentityApp.Infrastructure.Options;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IdentityApp.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly AuthOptions _authOptions;

        public AccountController(IMediator mediator, IOptionsMonitor<AuthOptions> optionsMonitor)
        {
            _mediator = mediator;
            _authOptions = optionsMonitor.CurrentValue;
        }

        /// <summary>
        /// Creates a user with given credentials.
        /// </summary>
        [HttpPost]
        [Route("api/account/registration")]
        public async Task<IActionResult> Register(CreateUserCommand createUserCommand)
        {
            var result = await _mediator.Send(createUserCommand);

            return result.Succeeded ? Ok(result) : StatusCode(500, result);
        }

        /// <summary>
        /// Logins a user with given credentials.
        /// </summary>
        [HttpPost]
        [Route("api/account/login")]
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

            var encodedJwt = JwtGenerator.Generate(_authOptions);

            HttpContext.Response.Cookies.Append(
                ".AspNetCore.Application.Id", // nonsense name for securing purposes.
                encodedJwt,
                new CookieOptions
                {
                    MaxAge = TimeSpan.FromMinutes(_authOptions.Lifetime),
                    HttpOnly = true,
                });

            return Ok(result);
        }
    }
}