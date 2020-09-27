using System;

namespace FoodStylesScraper.Data.Models
{
    public class ScrapedMenuItem
    {
        public Guid Id { get; set; }
        public string MenuTitle { get; set; }
        public string MenuDescription { get; set; }
        public string MenuSectionTitle { get; set; }
        public string DishName { get; set; }
        public string DishDescription { get; set; }
    }
}
