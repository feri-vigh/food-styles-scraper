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
            menuUrl = RemoveTrailingSlash(menuUrl);

            using (var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
            {
                driver.Manage().Window.Size = new Size(1600, 1200);
                driver.Navigate().GoToUrl(menuUrl);

                var menuLinks = GetMenuLinks(driver);
                menuLinks.Remove(menuUrl);
                menuLinks.Insert(0, menuUrl);

                logger.LogInformation("Located menu links.");

                var result = new List<MenuItemDto>();

                foreach (var menuLink in menuLinks)
                {
                    if (RemoveTrailingSlash(driver.Url) != menuLink)
                        driver.Navigate().GoToUrl(menuLink);

                    logger.LogInformation($"Porcessing {menuLink}...");

                    var activeMenuTitle = GetActiveMenuItemText(driver);
                    var menuDescription = GetMenuDescription(driver);

                    var mainContent = GetMainContent(driver);
                    if (mainContent == null)
                        return new List<MenuItemDto>();

                    var menuTitles = GetMenuTitles(mainContent);
                    var detailPageUrls = new Dictionary<string, string>();

                    if (menuTitles.Any())
                    {
                        foreach (var menuTitle in menuTitles)
                        {
                            var menuSectionTitle = GetMenuTitleText(menuTitle);
                            var menuSectionId = GetMenuSectionId(menuTitle);

                            var dishes = GetMenuSectionDishesBySectionId(mainContent, menuSectionId);

                            foreach (var dish in dishes)
                            {
                                var dishName = dish.GetAttribute("title");
                                if (string.IsNullOrWhiteSpace(dishName))
                                    continue;

                                var dto = new MenuItemDto(activeMenuTitle, menuDescription, dishName) { MenuSectionTitle = menuSectionTitle, };

                                detailPageUrls.Add(dto.DishName, dish.GetAttribute("href"));

                                result.Add(dto);
                            }
                        }
                    }
                    else
                    {
                        var dishes = GetMenuSectionDishes(mainContent);

                        foreach (var dish in dishes)
                        {
                            var dishName = dish.GetAttribute("title");
                            if (string.IsNullOrWhiteSpace(dishName))
                                continue;

                            var dto = new MenuItemDto(activeMenuTitle, menuDescription, dishName);

                            detailPageUrls.Add(dto.DishName, dish.GetAttribute("href"));

                            result.Add(dto);
                        }
                    }

                    foreach (var dto in result)
                    {
                        var url = detailPageUrls.ContainsKey(dto.DishName) ? detailPageUrls[dto.DishName] : null;
                        if (string.IsNullOrWhiteSpace(url))
                            continue;

                        dto.DishDescription = GetDishDescription(driver, url);
                    }

                    logger.LogInformation($"Porcessed {menuLink}.");
                }

                return result;
            }
        }

        private List<string> GetMenuLinks(ChromeDriver driver)
        {
            try
            {
                var menuItems = driver.FindElements(By.XPath("//nav[contains(@class, 'navbar')]//ul[contains(@class, 'submenu')]//li//a"));

                logger.LogInformation($"Located menu items.");

                return menuItems.Select(mi => mi.GetAttribute("href")).Select(url => RemoveTrailingSlash(url)).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error locating menu items. {ex.Message}", ex);
                return new List<string>();
            }
        }

        private string RemoveTrailingSlash(string url)
        {
            if (!string.IsNullOrEmpty(url) && url.EndsWith(@"/"))
                url = url.Substring(0, url.Length - 1);

            return url;
        }

        private string GetActiveMenuItemText(ChromeDriver driver)
        {
            try
            {
                var activeMenu = driver.FindElement(By.XPath("//nav[contains(@class, 'navbar')]//ul[contains(@class, 'submenu')]//li//a[contains(@class, 'active')]"));

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

        private List<IWebElement> GetMenuSectionDishesBySectionId(IWebElement mainContent, string menuSectionId)
        {
            try
            {
                var anchors = mainContent.FindElements(By.XPath($"//div[contains(@id, '{menuSectionId}')]//div[contains(@class, 'menu-item')]//a"));

                logger.LogInformation($"Located menu section anchors for section id = {menuSectionId}.");

                return anchors.ToList();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error locating menu section anchors for section id = {menuSectionId}. {ex.Message}", ex);
                return new List<IWebElement>();
            }
        }

        private List<IWebElement> GetMenuSectionDishes(IWebElement mainContent)
        {
            try
            {
                var anchors = mainContent.FindElements(By.XPath("//div[contains(@class, 'menu-item')]//a"));

                logger.LogInformation($"Located menu section anchors.");

                return anchors.ToList();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error locating menu section anchors. {ex.Message}", ex);
                return new List<IWebElement>();
            }
        }

        private string GetDishDescription(ChromeDriver driver, string url)
        {
            try
            {
                driver.Navigate().GoToUrl(url);

                var descriptionParagraph = driver.FindElement(By.XPath("//main[contains(@class, 'main-content')]//article[contains(@class, 'menu-item-details')]//div//p"));

                logger.LogInformation($"Navigated to details page and got description for Url = {url}.");

                return descriptionParagraph.Text;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error getting dish description. {ex.Message}", ex);
                return string.Empty;
            }
        }
    }
}
