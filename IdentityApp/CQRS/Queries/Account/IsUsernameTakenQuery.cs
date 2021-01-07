using IdentityApp.Infrastructure.Helpers.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityApp.CQRS.Queries.Account
{
    public class IsUsernameTakenQuery : IRequest<OperationDataResult<bool>>
    {
        [Required(ErrorMessage = "What is your username?")]
        [MaxLength(32, ErrorMessage = "Must be 32 characters or less!")]
        [MinLength(3, ErrorMessage = "Must be 3 characters or more!")]
        public string Username { get; set; }
    }

    public class IsUsernameTakenQueryHandler : IRequestHandler<IsUsernameTakenQuery, OperationDataResult<bool>>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IsUsernameTakenQueryHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<OperationDataResult<bool>> Handle(IsUsernameTakenQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var identityUser = await _userManager.FindByNameAsync(request.Username);

                if (identityUser == null)
                {
                    return new OperationDataResult<bool>(true, false);
                }

                return new OperationDataResult<bool>(true, true);

            }
            catch (Exception e)
            {
                return new OperationDataResult<bool>(false, new List<string> { e.Message }, false);
            }
        }
    }
}
