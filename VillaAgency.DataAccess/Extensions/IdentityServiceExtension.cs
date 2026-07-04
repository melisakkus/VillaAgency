using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VillaAgency.DataAccess.Configurations;
using VillaAgency.Entity.Identity;
using Microsoft.AspNetCore.Identity;


namespace VillaAgency.DataAccess.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,IConfiguration config)
        {
            var mongodbSettings = config.GetSection("MongoDB").Get<MongoDbSettings>()
                                ?? throw new InvalidOperationException("MongoDB configuration is missing.");
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            })
                .AddMongoDbStores<AppUser, AppRole, string>(
                mongodbSettings.ConnectionString, mongodbSettings.DatabaseName)
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
