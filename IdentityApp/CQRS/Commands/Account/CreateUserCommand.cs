using IdentityApp.Infrastructure.Helpers.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityApp.CQRS.Commands.Account
{
    public class CreateUserCommand : IRequest<OperationDataResult<bool>>
    {
        [Required(ErrorMessage = "What is your username?")]
        [MaxLength(32, ErrorMessage = "Must be 32 characters or less!")]
        [MinLength(3, ErrorMessage = "Must be 3 characters or more!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "What is your password?")]
        [MaxLength(32, ErrorMessage = "Must be 32 characters or less!")]
        [MinLength(6, ErrorMessage = "Must be 6 characters or more!")]
        public string Password { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, OperationDataResult<bool>>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public CreateUserCommandHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<OperationDataResult<bool>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var identityResult =
                    await _userManager.CreateAsync(new IdentityUser(request.Username), request.Password);

                if (!identityResult.Succeeded)
                {
                    //return new OperationResult(true, identityResult.Errors.Select(error => error.Description));
                    return new OperationDataResult<bool>(true, identityResult.Errors.Select(error => error.Description), false);
                }

                return new OperationDataResult<bool>(true, true);
            }
            catch (Exception e)
            {
                //return new OperationResult(false, new List<string> { e.Message });
                return new OperationDataResult<bool>(false, new List<string> { e.Message });
            }
        }
    }
}
