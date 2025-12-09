using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _2_30_3_1
{
    [TestMethod]
    public void Dashboard_CompactSummaryVisible()
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

            var summary = driver.FindElement(By.Id("dashboard-summary"));

            if (!summary.Displayed)
                throw new Exception("Compact summary is not visible.");

            Console.WriteLine("PASS: Compact summary visible.");
        }
        finally
        {
            driver?.Quit();
        }
    }
}
