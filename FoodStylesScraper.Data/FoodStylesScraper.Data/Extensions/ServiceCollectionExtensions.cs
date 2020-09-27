using FoodStylesScraper.Contracts;
using FoodStylesScraper.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FoodStylesScraper.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFoodStylesScraperData(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<FoodStyleScraperContext>(options => options
                .UseSqlServer(connectionString, builder => builder.MigrationsAssembly(typeof(FoodStyleScraperContext).Assembly.FullName)));

            services.AddScoped<IScrapedMenuItemRepository, ScrapedMenuItemRepository>();

            return services;
        }
    }
}
