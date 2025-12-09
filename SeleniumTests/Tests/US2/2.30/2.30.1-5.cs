using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _2_30_1_5
{
    [TestMethod]
    public void Dashboard_NameMatchesDatabase()
    {
        IWebDriver driver = null;

        try
        {
            driver = new ChromeDriver();
            var loginPage = new SeleniumTests.Pages.LoginPage(driver);

            // Expected name from database for this specific account
            string expectedName = "John Doe"; // Change according to your test database

            // Step 1: Navigate
            loginPage.Navigate();

            // Step 2: Login with patient account whose name exists in DB
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();

            // Step 3: Wait for dashboard
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.FindElement(By.Id("dashboard-container")));

            // Step 4: Retrieve welcome message
            var message = driver.FindElement(By.XPath("//*[contains(text(),'Welkom')]"));

            if (!message.Text.Contains(expectedName))
            {
                Console.WriteLine($"FAIL: Expected '{expectedName}', got '{message.Text}'");
                throw new Exception("Displayed name does not match database name.");
            }

            Console.WriteLine("PASS: Welcome message matches database name.");
        }
        finally
        {
            driver?.Quit();
        }
    }
}
