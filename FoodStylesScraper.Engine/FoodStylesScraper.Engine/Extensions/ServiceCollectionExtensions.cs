using FoodStylesScraper.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace FoodStylesScraper.Engine.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFoodStylesScrapingEngine(this IServiceCollection services)
        {
            services.AddScoped<IScrapingEngine, ScrapingEngine>();

            return services;
        }
    }
}
