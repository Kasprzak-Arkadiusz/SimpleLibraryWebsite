using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace SimpleLibraryWebsite.Models.Authorization
{
    public class LoanReaderAuthorizationHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, Loan>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Loan resource)
        {
            if (context.User.HasClaim(ClaimTypes.NameIdentifier, resource.ReaderId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
