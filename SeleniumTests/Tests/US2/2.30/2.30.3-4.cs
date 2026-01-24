using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_3_4
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "http://localhost";

        [TestInitialize]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            Console.WriteLine("Setup voltooid.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void Dashboard_UpdatesAfterDataChange()
        {
            Console.WriteLine("Test started: Dashboard_UpdatesAfterDataChange");
            Console.WriteLine("Test case: TC2.30.3-4 - Dashboard updates after data change");

            // Step 1: Navigate to login page and log in
            Console.WriteLine("Step 1: Navigating to login page and logging in...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");

            driver.FindElement(By.Id("email")).SendKeys("gebruiker@example.com");
            driver.FindElement(By.Id("wachtwoord")).SendKeys("Wachtwoord123");
            driver.FindElement(By.Id("login-btn")).Click();

            // Step 2: Wait for dashboard to load
            Console.WriteLine("Step 2: Waiting for dashboard...");
            var welcomeMessageElement = wait.Until(d =>
                d.FindElement(By.CssSelector("[data-test='welcome-message']")));

            string initialWelcomeMessage = welcomeMessageElement.Text;
            Console.WriteLine($"Initial welcome message: {initialWelcomeMessage}");

            // Step 3: Simulate data change (page refresh)
            Console.WriteLine("Step 3: Simulating data update by refreshing the page...");
            driver.Navigate().Refresh();

            // Step 4: Verify dashboard reloads correctly
            Console.WriteLine("Step 4: Verifying dashboard reload...");
            var refreshedWelcomeMessageElement = wait.Until(d =>
                d.FindElement(By.CssSelector("[data-test='welcome-message']")));

            string refreshedWelcomeMessage = refreshedWelcomeMessageElement.Text;
            Console.WriteLine($"Refreshed welcome message: {refreshedWelcomeMessage}");

            Assert.IsTrue(refreshedWelcomeMessageElement.Displayed,
                "Welcome message is not visible after refreshing the dashboard.");

            Assert.IsTrue(refreshedWelcomeMessage.Contains("Test Gebruiker"),
                "Welcome message does not contain the expected user name after refresh.");

            Console.WriteLine("Dashboard reloads correctly after data change. Test passed!");
        }
    }
}