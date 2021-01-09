using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using IdentityApp.CQRS.Queries.QueryResults;
using IdentityApp.Infrastructure.Helpers.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityApp.CQRS.Queries
{
    public class IsUsernameTakenQuery : IRequest<OperationDataResult<IsUsernameTakenQueryResult>>
    {
        [Required(ErrorMessage = "What is your username?")]
        [MaxLength(32, ErrorMessage = "Must be 32 characters or less!")]
        [MinLength(3, ErrorMessage = "Must be 3 characters or more!")]
        public string Username { get; set; }
    }

    public class IsUsernameTakenQueryHandler : IRequestHandler<IsUsernameTakenQuery, OperationDataResult<IsUsernameTakenQueryResult>>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IsUsernameTakenQueryHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<OperationDataResult<IsUsernameTakenQueryResult>> Handle(IsUsernameTakenQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var identityUser = await _userManager.FindByNameAsync(request.Username);

                if (identityUser == null)
                {
                    return new OperationDataResult<IsUsernameTakenQueryResult>(true, new IsUsernameTakenQueryResult(false));
                }

                return new OperationDataResult<IsUsernameTakenQueryResult>(true, new IsUsernameTakenQueryResult(true));

            }
            catch (Exception e)
            {
                return new OperationDataResult<IsUsernameTakenQueryResult>(false, new List<string> { e.Message }, new IsUsernameTakenQueryResult(false));
            }
        }
    }
}
