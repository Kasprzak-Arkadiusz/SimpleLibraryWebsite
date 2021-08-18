using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Models;
using SimpleLibraryWebsite.Models.Authorization;

[assembly: HostingStartup(typeof(SimpleLibraryWebsite.Areas.Identity.IdentityHostingStartup))]
namespace SimpleLibraryWebsite.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddIdentity<User, IdentityRole>(
                            options =>
                            {
                                options.SignIn.RequireConfirmedAccount = true;
                                options.User.RequireUniqueEmail = true;
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