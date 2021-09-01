using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Services.Authorization
{
    public class LoanLibrarianAuthorizationHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, Loan>
    {
        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                OperationAuthorizationRequirement requirement,
                Loan resource)
        {
            if (resource == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name != Constants.ReturnOperationName &&
                requirement.Name != Constants.ReadOperationName)
            {
                return Task.CompletedTask;
            }

            if (context.User.IsInRole(Role.Librarian.ToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
