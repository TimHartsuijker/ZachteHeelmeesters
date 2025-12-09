using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumTests;

[TestClass]
public class _2_30_3_3
{
    [TestMethod]
    public void Dashboard_OverviewReadable()
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

            driver.FindElement(By.Id("dashboard-summary"));

            // Example checks:
            bool hasSections = driver.FindElements(By.ClassName("summary-section")).Count >= 1;

            if (!hasSections)
                throw new Exception("Summary lacks grouping/sections.");

            Console.WriteLine("PASS: Dashboard overview is structured and readable.");
        }
        finally
        {
            driver?.Quit();
        }
    }
}
