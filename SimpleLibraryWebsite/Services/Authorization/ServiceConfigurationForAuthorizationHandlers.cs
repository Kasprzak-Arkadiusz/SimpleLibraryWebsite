using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Services.Authorization
{
    public static class ServiceConfigurationForAuthorizationHandlers
    {
        public static void Configure(IServiceCollection services)
        {
            //Administrator handlers
            services.AddSingleton<IAuthorizationHandler,
                AdministratorsAuthorizationHandler<Book>>();

            services.AddSingleton<IAuthorizationHandler,
                AdministratorsAuthorizationHandler<Loan>>();

            services.AddSingleton<IAuthorizationHandler,
                AdministratorsAuthorizationHandler<Request>>();

            //Librarian handlers
            services.AddSingleton<IAuthorizationHandler,
                LoanLibrarianAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler,
                BookLibrarianAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler,
                RequestLibrarianAuthorizationHandler>();

            //Reader handlers
            services.AddSingleton<IAuthorizationHandler,
                BookReaderAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler,
                RequestReaderAuthorizationHandler>();
        }
    }
}
