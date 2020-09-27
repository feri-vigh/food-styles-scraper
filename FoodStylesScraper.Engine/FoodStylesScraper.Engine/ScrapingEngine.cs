using FoodStylesScraper.Contracts;
using FoodStylesScraper.Dto;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FoodStylesScraper.Engine
{
    public class ScrapingEngine : IScrapingEngine
    {
        private readonly ILogger<ScrapingEngine> logger;

        public ScrapingEngine(ILogger<ScrapingEngine> logger)
        {
            this.logger = logger;
        }

        public List<MenuItemDto> ScrapeMenu(string menuUrl)
        {
            using (var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
            {
                driver.Manage().Window.Size = new Size(1600, 1200);
                driver.Navigate().GoToUrl(menuUrl);

                var result = new List<MenuItemDto>();

                var activeMenuTitle = GetActiveMenuItemText(driver);
                var menuDescription = GetMenuDescription(driver);

                var mainContent = GetMainContent(driver);
                if (mainContent == null)
                    return new List<MenuItemDto>();

                var menuTitles = GetMenuTitles(mainContent);

                foreach (var menuTitle in menuTitles)
                {
                    var menuSectionTitle = GetMenuTitleText(menuTitle);
                    var menuSectionId = GetMenuSectionId(menuTitle);

                    var dishes = GetMenuSectionDishes(mainContent, menuSectionId);

                    foreach (var dish in dishes)
                    {
                        var dto = new MenuItemDto
                        {
                            MenuTitle = activeMenuTitle,
                            MenuDescription = menuDescription,
                            MenuSectionTitle = menuSectionTitle,
                            DishName = dish.GetAttribute("title"),
                            DishDescription = GetMenuSectionDishDescription(dish)
                        };

                        result.Add(dto);
                    }
                }

                return result;
            }
        }

        private string GetActiveMenuItemText(ChromeDriver driver)
        {
            try
            {
                var navbar = driver.FindElement(By.ClassName("navbar"));
                var subMenu = navbar.FindElement(By.ClassName("submenu"));
                var activeMenu = subMenu.FindElement(By.ClassName("active"));

                logger.LogInformation($"Located active menu item with Text = {activeMenu.Text}");

                return activeMenu.Text;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error locating active menu item. {ex.Message}", ex);
                return string.Empty;
            }
        }

        private string GetMenuDescription(ChromeDriver driver)
        {
            try
            {
                var menuHeader = driver.FindElement(By.ClassName("menu-header"));
                var menuDescriptionParagraph = menuHeader.FindElement(By.XPath("//p"));

                logger.LogInformation($"Located active menu item with Text = {menuDescriptionParagraph.Text}");

                return menuDescriptionParagraph.Text;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error locating menu description. {ex.Message}", ex);
                return string.Empty;
            }
        }

        private IWebElement GetMainContent(ChromeDriver driver)
        {
            try
            {
                var mainContent = driver.FindElement(By.XPath("//main[contains(@class, 'main-content')]"));

                logger.LogInformation($"Located main content.");

                return mainContent;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error locating menu titles. {ex.Message}", ex);
                return null;
            }
        }

        private List<IWebElement> GetMenuTitles(IWebElement mainContent)
        {
            try
            {
                var menuTitles = mainContent.FindElements(By.XPath("//h4[contains(@class, 'menu-title')]"));

                logger.LogInformation($"Located menu items");

                return menuTitles.ToList();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error locating menu titles. {ex.Message}", ex);
                return new List<IWebElement>();
            }
        }

        private string GetMenuTitleText(IWebElement menuTitle)
        {
            try
            {
                var menuTitleSpan = menuTitle.FindElement(By.XPath("a//span"));

                logger.LogInformation($"Located menu title span with Text = {menuTitleSpan.Text}");

                return menuTitleSpan.Text;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error locating menu title span. {ex.Message}", ex);
                return string.Empty;
            }
        }

        private string GetMenuSectionId(IWebElement menuTitle)
        {
            try
            {
                var menuTitleAnchor = menuTitle.FindElement(By.XPath("a"));
                var id = menuTitleAnchor.GetAttribute("href");
                
                if (string.IsNullOrWhiteSpace(id))
                    return string.Empty;

                if (id.Contains(@"/"))
                    id = id.Substring(id.LastIndexOf(@"/") + 1);

                if (id.Contains("#"))
                    id = id.Replace("#", string.Empty);

                logger.LogInformation($"Located menu section id = {id} on menu section title.");

                return id;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error locating menu section id on menu section title. {ex.Message}", ex);
                return string.Empty;
            }
        }

        private List<IWebElement> GetMenuSectionDishes(IWebElement mainContent, string menuSectionId)
        {
            try
            {
                var anchors = mainContent.FindElements(By.XPath($"//div[contains(@id, '{menuSectionId}')]//div[contains(@class, 'menu-item')]//a"));

                logger.LogInformation($"Located menu section anchors.");

                return anchors.ToList();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error locating menu section anchors for section id = {menuSectionId}. {ex.Message}", ex);
                return new List<IWebElement>();
            }
        }

        private string GetMenuSectionDishDescription(IWebElement dishElement)
        {
            // TODO - click on element and get description
            return string.Empty;
        }
    }
}
