using IdentityApp.Infrastructure.Helpers.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityApp.Infrastructure.CQRS.Commands
{
    public class CreateUserCommand : IRequest<OperationResult>
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, OperationResult>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public CreateUserCommandHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<OperationResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var identityResult =
                    await _userManager.CreateAsync(new IdentityUser(request.Username), request.Password);

                if (!identityResult.Succeeded)
                {
                    return new OperationResult(true, identityResult.Errors.Select(error => error.Description));
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
