﻿using IdentityApp.Abstractions;
using IdentityApp.Infrastructure.Helpers.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityApp.CQRS.Commands
{
    public class LoginUserCommand : IRequest<OperationDataResult<string>>
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

    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, OperationDataResult<string>>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IJwtGenerator _jwtGenerator;

        public LoginUserCommandHandler(UserManager<IdentityUser> userManager, IJwtGenerator jwtGenerator)
        {
            _userManager = userManager;
            _jwtGenerator = jwtGenerator;
        }
        public async Task<OperationDataResult<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var foundIdentity = await _userManager.FindByNameAsync(request.Username);

                var identityResult =
                    await _userManager.CheckPasswordAsync(foundIdentity, request.Password);

                if (!identityResult)
                {
                    return new OperationDataResult<string>(true, new List<string> { "Incorrect credentials." });
                }

                var jwt = _jwtGenerator.GenerateJwt();
                return new OperationDataResult<string>(true, jwt);
            }
            catch (Exception e)
            {
                return new OperationDataResult<string>(false, new List<string> { e.Message });
            }
        }
    }
}
