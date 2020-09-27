using FoodStylesScraper.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FoodStylesScraper.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScrapeController : ControllerBase
    {
        private readonly ILogger<ScrapeController> logger;

        public ScrapeController(ILogger<ScrapeController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody] ScrapeParameters parameters)
        {
            try
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

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error scraping menu at {parameters.MenuUrl}", ex);
                return BadRequest();
            }
        }
    }

    public class ScrapeParameters
    {
        public string MenuUrl { get; set; }
    }
}
