using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Models;
using SimpleLibraryWebsite.Services.Authorization;

[assembly: HostingStartup(typeof(SimpleLibraryWebsite.Areas.Identity.IdentityHostingStartup))]
namespace SimpleLibraryWebsite.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices( services=>
            {
                services.AddIdentity<User, IdentityRole>(
                            options =>
                            {
                                options.Password.RequiredLength = 7;
                                options.Password.RequireDigit = false;
                                options.Password.RequireUppercase = false;
                                options.SignIn.RequireConfirmedAccount = true;
                                options.User.RequireUniqueEmail = true;
                                options.SignIn.RequireConfirmedEmail = true;
                            })
                    .AddRoles<IdentityRole>()
                        .AddEntityFrameworkStores<ApplicationDbContext>()
                        .AddDefaultUI()
                        .AddDefaultTokenProviders();

                services.AddAuthorization(options =>
                {
                    options.FallbackPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                });

                ServiceConfigurationForAuthorizationHandlers.Configure(services);
            });
        }
    }
}