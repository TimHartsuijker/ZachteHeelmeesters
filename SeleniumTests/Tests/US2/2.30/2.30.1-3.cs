using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _2_30_1_3
{
    [TestMethod]
    public void Dashboard_WelcomeMessageFirstAndRepeatedLogin()
    {
        IWebDriver driver = null;

        try
        {
            driver = new ChromeDriver();
            var loginPage = new SeleniumTests.Pages.LoginPage(driver);

            // Function to perform login and check welcome message
            void LoginAndCheck()
            {
                loginPage.Navigate();
                loginPage.EnterEmail("patient@example.com");
                loginPage.EnterPassword("Test123!");
                loginPage.ClickLogin();

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => driver.FindElement(By.Id("dashboard-container")));

                var message = driver.FindElement(By.XPath("//*[contains(text(),'Welkom')]"));

                if (!message.Displayed)
                    throw new Exception("Welcome message not visible after login.");

                Console.WriteLine("PASS: Welcome message visible.");
            }

            Console.WriteLine("LOG First login...");
            LoginAndCheck();

            // Logout
            // Replace selector with your real logout button
            driver.FindElement(By.Id("logout-button")).Click();
            Console.WriteLine("LOG Logged out.");

            // Second login
            Console.WriteLine("LOG Second login...");
            LoginAndCheck();

            Console.WriteLine("Test completed successfully.");
        }
        finally
        {
            driver?.Quit();
        }
    }
}
