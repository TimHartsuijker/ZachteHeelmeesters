using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _2_30_1_2
{
    [TestMethod]
    public void Dashboard_PersonalizedWelcomeMessage()
    {
        IWebDriver driver = null;

        try
        {
            Console.WriteLine("LOG Start test: Personalized welcome message");

            driver = new ChromeDriver();
            var loginPage = new SeleniumTests.Pages.LoginPage(driver);

            // Step 1: Navigate to login page
            loginPage.Navigate();

            // Step 2: Enter valid patient credentials
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");

            // Step 3: Login
            loginPage.ClickLogin();

            // Step 4: Wait for dashboard
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.FindElement(By.Id("dashboard-container")));

            // Step 5: Check for personalized welcome message
            // Example text: “Welkom John”
            var welcomeMessage = driver.FindElement(By.XPath("//*[contains(text(),'Welkom')]"));

            string expectedName = "John Doe"; // <-- Adjust for your test patient

            if (!welcomeMessage.Text.Contains(expectedName))
            {
                Console.WriteLine($"FAIL: Expected welcome message to contain patient name '{expectedName}'.");
                throw new Exception("Personalized name not found in welcome message.");
            }

            Console.WriteLine("PASS: Personalized welcome message detected.");
        }
        finally
        {
            driver?.Quit();
        }
    }
}
