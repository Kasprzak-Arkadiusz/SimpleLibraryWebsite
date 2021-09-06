using System.Collections.Generic;

namespace SimpleLibraryWebsite.Models.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string UserName { get; init; }
        public string Email { get; init; }
        public IEnumerable<string> Roles { get; init; }
    }
}
