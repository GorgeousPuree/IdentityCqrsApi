﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Infrastructure.Helpers.Responses;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Infrastructure.Helpers.Extensions
{
    public static class ApplicationExtensions
    {
        public static void CustomInvalidModelStateResponse(this ApiBehaviorOptions apiBehaviorOptions)
        {
            apiBehaviorOptions.InvalidModelStateResponseFactory = actionContext =>
            {
                OperationResult result = new OperationResult(false, new List<string>());

                foreach (var modelState in actionContext.ModelState)
                {
                    result.Messages = modelState.Value.Errors.Select(error => error.ErrorMessage).ToList();
                }

                return new BadRequestObjectResult(result);
            };
        }
    }
}
