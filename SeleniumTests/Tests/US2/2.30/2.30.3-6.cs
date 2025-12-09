using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumTests;

[TestClass]
public class _2_30_3_6
{
    [TestMethod]
    public void Dashboard_NoDataDisplaysFriendlyMessage()
    {
        IWebDriver driver = null;

        try
        {
            driver = new ChromeDriver();
            var loginPage = new SeleniumTests.Pages.LoginPage(driver);

            // Login with account with NO data
            loginPage.Navigate();
            loginPage.EnterEmail("emptydata@example.com");
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();

            var noAppointments = driver.FindElement(By.Id("no-appointments-message")).Text;
            var noMessages = driver.FindElement(By.Id("no-messages-message")).Text;

            if (!noAppointments.Contains("Geen aankomende afspraken"))
                throw new Exception("Missing appointments message incorrect.");

            if (!noMessages.Contains("Geen nieuwe berichten"))
                throw new Exception("Missing messages message incorrect.");

            Console.WriteLine("PASS: Dashboard correctly displays empty-state messages.");
        }
        finally
        {
            driver?.Quit();
        }
    }
}
