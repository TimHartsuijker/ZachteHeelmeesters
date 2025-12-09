using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _2_30_2_2
{
    [TestMethod]
    public void NavigationMenu_ContainsCorrectOptions()
    {
        IWebDriver driver = null;

        try
        {
            Console.WriteLine("LOG Start test: Navigation menu contains correct items");

            driver = new ChromeDriver();
            var loginPage = new SeleniumTests.Pages.LoginPage(driver);

            // Login
            loginPage.Navigate();
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();

            // Wait for dashboard
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.Id("dashboard-container")));

            // Expected menu items
            string[] expectedItems =
            {
                "Dashboard",
                "Afspraken",
                "Facturen",
                "Medisch dossier"
            };

            // Locate menu
            var navMenu = driver.FindElement(By.Id("navigation-menu"));

            foreach (string item in expectedItems)
            {
                var element = navMenu.FindElement(By.XPath($".//*[contains(text(),'{item}')]"));

                if (!element.Displayed)
                    throw new Exception($"Menu item '{item}' not found.");

                Console.WriteLine($"PASS: Menu item '{item}' found.");
            }
        }
        finally
        {
            driver?.Quit();
        }
    }
}
