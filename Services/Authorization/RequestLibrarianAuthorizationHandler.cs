using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace SimpleLibraryWebsite.Models.Authorization
{
    public class RequestLibrarianAuthorizationHandler  :
        AuthorizationHandler<OperationAuthorizationRequirement, Request>
    {
        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                OperationAuthorizationRequirement requirement,
                Request resource)
        {
            if (resource == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name != Constants.DeleteOperationName
            && requirement.Name != Constants.CreateOperationName)
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
