using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _2_30_1_4
{
    [TestMethod]
    public void Dashboard_WelcomeMessageWithoutName()
    {
        IWebDriver driver = null;

        try
        {
            driver = new ChromeDriver();
            var loginPage = new SeleniumTests.Pages.LoginPage(driver);

            // Step 1: Navigate to login
            loginPage.Navigate();

            // Step 2: Enter credentials for patient with NO name stored in DB
            loginPage.EnterEmail("noname@example.com");
            loginPage.EnterPassword("Test123!");

            // Step 3: Login
            loginPage.ClickLogin();

            // Step 4: Wait for dashboard
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.FindElement(By.Id("dashboard-container")));

            // Step 5: Check welcome message
            var welcomeMessage = driver.FindElement(By.XPath("//*[contains(text(),'Welkom')]"));

            string text = welcomeMessage.Text;

            // Example: “Welkom!” instead of “Welkom John”
            if (text.Contains("@") || text.Contains("null"))
                throw new Exception("Invalid fallback welcome message.");

            // Ensure patient name is NOT present
            if (text.Split(' ').Length > 1)
            {
                Console.WriteLine($"FAIL: Unexpected name found in fallback message: {text}");
                throw new Exception("Name should not appear in message.");
            }

            Console.WriteLine("PASS: Welcome message shown without patient name.");
        }
        finally
        {
            driver?.Quit();
        }
    }
}
