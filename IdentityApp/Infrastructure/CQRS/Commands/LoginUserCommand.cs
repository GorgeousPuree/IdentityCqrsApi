using IdentityApp.Infrastructure.Helpers.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityApp.Infrastructure.CQRS.Commands
{
    public class LoginUserCommand : IRequest<OperationResult>
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, OperationResult>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public LoginUserCommandHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<OperationResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var foundIdentity = await _userManager.FindByNameAsync(request.Username);

                var identityResult =
                    await _userManager.CheckPasswordAsync(foundIdentity, request.Password);

                if (!identityResult)
                {
                    return new OperationResult(true, new List<string> { "Incorrect credentials." });
                }

                return new OperationResult(true);
            }
            catch (Exception e)
            {
                return new OperationResult(false, new List<string> { e.Message });
            }
        }
    }
}
