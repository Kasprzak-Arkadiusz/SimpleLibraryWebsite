using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using SimpleLibraryWebsite.Services.Authorization;

namespace SimpleLibraryWebsite.Models.Authorization
{
    public class RequestReaderAuthorizationHandler:
        AuthorizationHandler<OperationAuthorizationRequirement, Request>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Request resource)
        {
            if (resource == null)
            {
                return Task.CompletedTask;
            }

            if (!context.User.HasClaim(ClaimTypes.NameIdentifier, resource.ReaderId))
            {
                return Task.CompletedTask;
            }

            if (requirement.Name != Constants.CreateOperationName 
            && requirement.Name != Constants.UpdateOperationName
            && requirement.Name != Constants.DeleteOperationName )
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
