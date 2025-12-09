using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _2_30_2_4
{
    [TestMethod]
    public void NavigationMenu_VisibleOnAllPages()
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

            string[] menuItems = { "Dashboard", "Afspraken", "Facturen", "Medisch dossier" };

            foreach (var item in menuItems)
            {
                driver.FindElement(By.XPath($"//*[contains(text(),'{item}')]")).Click();

                // Ensure menu is still visible
                var navMenu = driver.FindElement(By.Id("navigation-menu"));

                if (!navMenu.Displayed)
                    throw new Exception($"Navigation menu disappeared on page: {item}");

                Console.WriteLine($"PASS: Navigation menu visible on '{item}' page.");
            }
        }
        finally
        {
            driver?.Quit();
        }
    }
}
