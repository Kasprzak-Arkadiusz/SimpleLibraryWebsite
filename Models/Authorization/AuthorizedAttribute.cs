using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace SimpleLibraryWebsite.Models
{

    public enum Role
    {
        Admin = 1,
        Librarian = 2,
        Reader = 3
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizeWithEnumRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeWithEnumRolesAttribute(params object[] roles)
        {
            if (roles.Any(r => r.GetType().BaseType != typeof(Enum)))
                throw new ArgumentException("The specified role is not of the Enum type.");

            Roles = string.Join(",", roles.Select(r => Enum.GetName(r.GetType(), r)));
        }
    }
}
