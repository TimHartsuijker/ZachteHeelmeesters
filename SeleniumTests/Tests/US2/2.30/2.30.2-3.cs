using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _2_30_2_3
{
    [TestMethod]
    public void NavigationMenu_LinksNavigateCorrectly()
    {
        IWebDriver driver = null;

        try
        {
            driver = new ChromeDriver();
            var loginPage = new SeleniumTests.Pages.LoginPage(driver);

            // Login
            loginPage.Navigate();
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.Id("navigation-menu")));

            // Menu items and their expected page IDs
            Dictionary<string, string> menuToPage = new()
            {
                { "Dashboard", "dashboard-container" },
                { "Afspraken", "appointments-page" },
                { "Facturen", "billing-page" },
                { "Medisch dossier", "medical-records-page" }
            };

            foreach (var item in menuToPage)
            {
                // Click menu item
                driver.FindElement(By.XPath($"//*[contains(text(),'{item.Key}')]")).Click();

                // Wait for expected page
                wait.Until(d => d.FindElement(By.Id(item.Value)));

                Console.WriteLine($"PASS: '{item.Key}' correctly navigates to '{item.Value}'.");
            }
        }
        finally
        {
            driver?.Quit();
        }
    }
}
