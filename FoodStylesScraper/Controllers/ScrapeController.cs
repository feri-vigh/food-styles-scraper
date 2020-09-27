using FoodStylesScraper.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace FoodStylesScraper.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScrapeController : ControllerBase
    {
        private readonly ILogger<ScrapeController> logger;
        private readonly IScrapingEngine scrapingEngine;

        public ScrapeController(
            ILogger<ScrapeController> logger,
            IScrapingEngine scrapingEngine)
        {
            this.logger = logger;
            this.scrapingEngine = scrapingEngine;
        }

        [HttpPost]
        public IActionResult Post([FromBody] ScrapeParameters parameters)
        {
            try
            {
                var results = scrapingEngine.ScrapeMenu(parameters.MenuUrl);

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
