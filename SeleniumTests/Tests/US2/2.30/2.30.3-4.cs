using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _2_30_3_4
{
    [TestMethod]
    public void Dashboard_UpdatesAfterDataChange()
    {
        IWebDriver driver = null;

        try
        {
            driver = new ChromeDriver();
            var loginPage = new SeleniumTests.Pages.LoginPage(driver);

            loginPage.Navigate();
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.Id("dashboard-summary")));

            // Backend inserts a new appointment here (manual simulation)

            driver.Navigate().Refresh();
            wait.Until(d => d.FindElement(By.Id("dashboard-summary")));

            var appointments = driver.FindElement(By.Id("summary-next-appointment")).Text;

            if (!appointments.Contains("NEW"))
                throw new Exception("Updated data not visible.");

            Console.WriteLine("PASS: Dashboard updates after new data.");
        }
        finally
        {
            driver?.Quit();
        }
    }
}
