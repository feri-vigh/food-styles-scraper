using FoodStylesScraper.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodStylesScraper.Data
{
    public class FoodStyleScraperContext : DbContext
    {
        public FoodStyleScraperContext(DbContextOptions<FoodStyleScraperContext> options) : base(options)
        {
        }

        public DbSet<ScrapedMenuItem> ScrapedMenuItems { get; set; }
    }
}
