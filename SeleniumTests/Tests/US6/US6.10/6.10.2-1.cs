using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class TC_6_10_2_1
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
        public void TC_6_10_2_1_VerifyAddAccountButtonIsVisible()
        {
            Console.WriteLine("Test started: TC_6_10_2_1_VerifyAddAccountButtonIsVisible");

            // Step 1: Navigate to login page
            driver.Navigate().GoToUrl($"{baseUrl}/");

            // Step 2: Login as administrative employee
            loginPage.EnterEmail("admin@example.com");
            loginPage.EnterPassword("AdminPassword123!");
            loginPage.ClickLogin();

            wait.Until(drv => drv.Url.Contains("dashboard"));
            Console.WriteLine("Admin successfully logged in.");

            // Step 3: Open account page
            driver.Navigate().GoToUrl($"{baseUrl}/user-management");
            Console.WriteLine("Account page opened.");

            // Step 4: Verify Add Account button is visible and enabled
            IWebElement addAccountButton = wait.Until(drv =>
                drv.FindElement(By.XPath("//button[contains(text(),'Add Account')]"))
            );

            Assert.IsTrue(addAccountButton.Displayed,
                "The 'Add Account' button is not visible.");

            Assert.IsTrue(addAccountButton.Enabled,
                "The 'Add Account' button is not clickable.");

            Console.WriteLine("'Add Account' button is visible and clickable.");
            Console.WriteLine("Test completed successfully.");
        }
    }
}
