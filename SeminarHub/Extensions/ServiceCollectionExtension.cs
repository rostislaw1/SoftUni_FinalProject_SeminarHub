using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension 
    {
        public static IServiceCollection AddApplicatoinService(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection AddApplicatoinDbContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<SeminarHubDbContext>(options =>
                options.UseSqlServer(connectionString));
           
            services.AddDatabaseDeveloperPageExceptionFilter();

            return services;
        }

        public static IServiceCollection AddApplicatoinIdentity(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddDefaultIdentity<IdentityUser>(options =>
                    {
                        options.SignIn.RequireConfirmedAccount = false;
                        options.Password.RequireDigit = false;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                    })
              .AddEntityFrameworkStores<SeminarHubDbContext>();
            services.AddControllersWithViews();
            return services;
        }
    }
}
