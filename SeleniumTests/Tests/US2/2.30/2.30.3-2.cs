using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _2_30_3_2
{
    [TestMethod]
    public void Dashboard_SummaryMatchesDatabase()
    {
        IWebDriver driver = null;

        try
        {
            driver = new ChromeDriver();
            var loginPage = new SeleniumTests.Pages.LoginPage(driver);

            // Expected DB values (sample)
            string expectedNextAppointment = "12-12-2025";
            string expectedUnreadMessages = "3";

            loginPage.Navigate();
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.Id("dashboard-summary")));

            var nextAppt = driver.FindElement(By.Id("summary-next-appointment")).Text;
            var unreadMsg = driver.FindElement(By.Id("summary-unread-messages")).Text;

            if (!nextAppt.Contains(expectedNextAppointment))
                throw new Exception("Next appointment does not match DB.");

            if (!unreadMsg.Contains(expectedUnreadMessages))
                throw new Exception("Unread messages do not match DB.");

            Console.WriteLine("PASS: Summary data matches database values.");
        }
        finally
        {
            driver?.Quit();
        }
    }
}
