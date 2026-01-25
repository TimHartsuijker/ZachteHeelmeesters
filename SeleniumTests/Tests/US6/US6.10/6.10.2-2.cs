using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class TC_6_10_2_2
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
        public void TC_6_10_2_2_VerifyAddAccountButtonNavigation()
        {
            Console.WriteLine("Test started: TC_6_10_2_2_VerifyAddAccountButtonNavigation");

            // Step 1: Navigate to login page
            driver.Navigate().GoToUrl($"{baseUrl}/");

            // Step 2: Login as administrative employee
            loginPage.EnterEmail("admin@example.com");
            loginPage.EnterPassword("AdminPassword123!");
            loginPage.ClickLoginButton();

            wait.Until(drv => drv.Url.Contains("dashboard"));
            Console.WriteLine("Admin successfully logged in.");

            // Step 3: Open account page
            driver.Navigate().GoToUrl($"{baseUrl}/user-management");
            Console.WriteLine("Account page opened.");

            // Step 4: Click Add Account button
            IWebElement addAccountButton = wait.Until(drv =>
                drv.FindElement(By.XPath("//button[contains(text(),'Add Account')]"))
            );

            addAccountButton.Click();
            Console.WriteLine("'Add Account' button clicked.");

            // Step 5: Verify navigation to create account page
            wait.Until(drv => drv.Url.Contains("create-user"));

            Assert.IsTrue(driver.Url.Contains("create-user"),
                "Clicking the 'Add Account' button did not navigate to the create account page.");

            Console.WriteLine("Create account page opened successfully.");
            Console.WriteLine("Test completed successfully.");
        }
    }
}
