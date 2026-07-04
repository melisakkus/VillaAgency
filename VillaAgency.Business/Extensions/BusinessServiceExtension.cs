using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VillaAgency.Business.Abstract;
using VillaAgency.Business.Concrete;
using VillaAgency.Business.Validators.BannerValidators;
using VillaAgency.DataAccess.Extension;
using VillaAgency.DataAccess.Extensions;

namespace VillaAgency.Business.Extension
{
    public static class BusinessServiceExtension
    {
        public static IServiceCollection AddBusinessServices(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddDataAccessServices(config);
            services.AddIdentityServices(config);

            services.AddScoped<IBannerService, BannerManager>();
            services.AddScoped<IContactService, ContactManager>();
            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<ICounterService, CounterManager>();
            services.AddScoped<IFeatureService, FeatureManager>();
            services.AddScoped<IMessageService, MessageManager>();
            services.AddScoped<IQuestionService, QuestionManager>();
            services.AddScoped<IVideoService, VideoManager>();

            services.AddScoped<IAuthService,AuthManager>();

            //services.AddSingleton<ICacheService, CacheManager>();

            services.AddValidatorsFromAssemblyContaining<CreateBannerValidator>();

            return services;
        }
    }
}
