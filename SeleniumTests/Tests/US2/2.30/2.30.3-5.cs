using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumTests;

[TestClass]
public class _2_30_3_5
{
    [TestMethod]
    public void Dashboard_ShowsGracefulErrorOnDataFailure()
    {
        IWebDriver driver = null;

        try
        {
            driver = new ChromeDriver();

            // Backend simulation: DB failure active

            var loginPage = new SeleniumTests.Pages.LoginPage(driver);
            loginPage.Navigate();
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();

            var errorMessage = driver.FindElement(By.Id("data-error-message"));

            if (!errorMessage.Text.Contains("Gegevens kunnen niet geladen worden"))
                throw new Exception("Expected user-friendly error not shown.");

            Console.WriteLine("PASS: Dashboard handles data errors correctly.");
        }
        finally
        {
            driver?.Quit();
        }
    }
}
