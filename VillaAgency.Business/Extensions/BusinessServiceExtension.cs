using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VillaAgency.DataAccess.Extension;

namespace VillaAgency.Business.Extension
{
    public static class BusinessServiceExtension
    {
        public static IServiceCollection AddBusinessServices(
            this IServiceCollection services, IConfiguration config)
        {
            // 1. Önce DataAccess katmanının kendi servislerini kaydetmesini sağlıyoruz
            services.AddDataAccessServices(config);

            // 2. Business katmanına ait Manager'ları (İş sınıflarını) buraya kaydediyoruz
            // Örnek: services.AddScoped<IVillaService, VillaManager>();

            return services;
        }
    }
}
