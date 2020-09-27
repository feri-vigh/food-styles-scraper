using FoodStylesScraper.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodStylesScraper.Contracts
{
    public interface IScrapedMenuItemRepository
    {
        Task AddOrUpdateScrapedMenuItemsAsync(List<MenuItemDto> scrapedMenuItems);
    }
}
