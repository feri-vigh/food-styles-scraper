﻿namespace FoodStylesScraper.Dto
{
    public class MenuItemDto
    {
        public string MenuTitle { get; set; }
        public string MenuDescription { get; set; }
        public string MenuSectionTitle { get; set; }
        public string DishName { get; set; }
        public string DishDescription { get; set; }

        public MenuItemDto(string menuTitle, string menuDescription, string dishName)
        {
            MenuTitle = menuTitle;
            MenuDescription = menuDescription;
            DishName = dishName;
        }
    }
}
