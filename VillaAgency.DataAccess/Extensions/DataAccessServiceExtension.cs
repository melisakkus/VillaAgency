using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.DataAccess.Concrete.MongoDb.Driver;
using VillaAgency.DataAccess.Configurations;
using VillaAgency.DataAccess.Context;

namespace VillaAgency.DataAccess.Extension
{
    public static class DataAccessServiceExtension
    {
        public static IServiceCollection AddDataAccessServices(
            this IServiceCollection services, IConfiguration config)
        {
            var mongoSettings = config.GetSection("MongoDB").Get<MongoDbSettings>()
                ?? throw new InvalidOperationException("MongoDB configuration is missing.");

            services.AddSingleton(mongoSettings);
            services.AddSingleton<MongoDbContext>();

            services.AddScoped(typeof(IGenericDal<>), typeof(GenericRepository<>));
            // Sadece özel metodu olan:
            // services.AddScoped<IProductDal, ProductDal>();
            return services;
        }
    }
}
