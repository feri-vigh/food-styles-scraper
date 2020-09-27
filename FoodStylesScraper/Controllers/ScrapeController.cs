using FoodStylesScraper.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FoodStylesScraper.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScrapeController : ControllerBase
    {
        private readonly ILogger<ScrapeController> logger;
        private readonly IScrapingEngine scrapingEngine;
        private readonly IScrapedMenuItemRepository scrapedMenuItemRepository;

        public ScrapeController(
            ILogger<ScrapeController> logger,
            IScrapingEngine scrapingEngine,
            IScrapedMenuItemRepository scrapedMenuItemRepository)
        {
            this.logger = logger;
            this.scrapingEngine = scrapingEngine;
            this.scrapedMenuItemRepository = scrapedMenuItemRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ScrapeParameters parameters)
        {
            try
            {
                var results = scrapingEngine.ScrapeMenu(parameters.MenuUrl);

                await scrapedMenuItemRepository.AddOrUpdateScrapedMenuItemsAsync(results);

                return Ok(results);
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
