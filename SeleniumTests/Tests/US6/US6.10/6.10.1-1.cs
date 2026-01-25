using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class TC_6_10_1_1
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "http://localhost:5173";

        private LoginPage loginPage;

        [TestInitialize]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            loginPage = new LoginPage(driver);
        }

        [TestCleanup]
        public void Cleanup()
        {
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void TC_6_10_1_1_AdminCanAccessCreateUserAccountPage()
        {
            Console.WriteLine("Test started: TC_6_10_1_1_AdminCanAccessCreateUserAccountPage");

            // Step 1: Navigate to login page
            Console.WriteLine("Navigating to login page...");
            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine("Navigation completed.");

            // Step 2: Log in as administrative employee
            Console.WriteLine("Logging in as administrative employee...");
            loginPage.EnterEmail("admin@example.com");
            loginPage.EnterPassword("AdminPassword123!");
            loginPage.ClickLogin();

            // Wait until dashboard or homepage is loaded
            wait.Until(drv => drv.Url.Contains("dashboard"));
            Console.WriteLine("Login successful.");

            // Step 3: Navigate to User Management page
            Console.WriteLine("Navigating to User Management...");
            driver.Navigate().GoToUrl($"{baseUrl}/user-management");

            // Step 4: Verify create user account page is displayed
            Console.WriteLine("Verifying create user account page is displayed...");
            IWebElement createUserHeader = wait.Until(drv =>
                drv.FindElement(By.XPath("//h1[contains(text(),'Create User')]"))
            );

            Assert.IsTrue(createUserHeader.Displayed,
                "The create user account page is not visible for the administrative employee.");

            Console.WriteLine("Create user account page is visible and accessible.");
            Console.WriteLine("Test completed successfully.");
        }
    }
}
