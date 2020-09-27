using FoodStylesScraper.Contracts;
using FoodStylesScraper.Dto;
using System.Collections.Generic;

namespace FoodStylesScraper.Engine
{
    public class ScrapingEngine : IScrapingEngine
    {
        public List<MenuItemDto> ScrapeMenu(string menuUrl)
        {
            var result = new List<MenuItemDto>
            {
                new MenuItemDto
                {
                    MenuTitle = "Breakfast",
                    MenuDescription = "Our nutritious breakfasts are served in seconds and last until lunch...",
                    MenuSectionTitle = "Super Eggs",
                    DishName = "Super Eggs",
                    DishDescription = "Free-range egg omelette, rolled and filled with avocado, roquito peppers, edamame, spinach and free-range egg mayonnaise."
                }
            };

            return result;
        }
    }
}
