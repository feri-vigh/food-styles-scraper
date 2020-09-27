using FoodStylesScraper.Contracts;
using FoodStylesScraper.Data.Models;
using FoodStylesScraper.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodStylesScraper.Data.Repositories
{
    public class ScrapedMenuItemRepository : IScrapedMenuItemRepository
    {
        private readonly FoodStyleScraperContext context;

        public ScrapedMenuItemRepository(FoodStyleScraperContext context)
        {
            this.context = context;
        }

        public async Task AddOrUpdateScrapedMenuItemsAsync(List<MenuItemDto> scrapedMenuItems)
        {
            var existingMenuItems = await context.ScrapedMenuItems.ToListAsync();

            foreach (var menuItem in scrapedMenuItems)
            {
                var existingMenuItem = existingMenuItems
                    .FirstOrDefault(i => i.MenuTitle == menuItem.MenuTitle && i.MenuSectionTitle == menuItem.MenuSectionTitle && i.DishName == menuItem.DishName);

                if (existingMenuItem == null)
                {
                    var newMenuItem = new ScrapedMenuItem
                    {
                        Id = Guid.NewGuid(),
                        MenuTitle = menuItem.MenuTitle,
                        MenuDescription = menuItem.MenuDescription,
                        MenuSectionTitle = menuItem.MenuSectionTitle,
                        DishName = menuItem.DishName,
                        DishDescription = menuItem.DishDescription
                    };

                    await context.ScrapedMenuItems.AddAsync(newMenuItem);
                }
                else
                {
                    existingMenuItem.DishDescription = menuItem.DishDescription;
                    existingMenuItem.MenuDescription = menuItem.MenuDescription;
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
