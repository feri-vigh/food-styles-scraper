using FoodStylesScraper.Dto;
using System.Collections.Generic;

namespace FoodStylesScraper.Contracts
{
    public interface IScrapingEngine
    {
        public List<MenuItemDto> ScrapeMenu(string menuUrl);
    }
}
