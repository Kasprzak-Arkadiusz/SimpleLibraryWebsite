using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Services.Authorization
{
    public class BookReaderAuthorizationHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, Book>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Book resource)
        {
            if (resource == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name != Constants.BorrowOperationName)
            {
                return Task.CompletedTask;
            }

            if (context.User.IsInRole(Role.Reader.ToString()))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
