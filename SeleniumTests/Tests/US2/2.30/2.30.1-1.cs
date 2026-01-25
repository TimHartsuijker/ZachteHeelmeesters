using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_1_1
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
        }

        [TestCleanup]
        public void Cleanup()
        {
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void TC_2_30_1_1_Dashboard_DisplaysWelcomeMessage()
        {
            Console.WriteLine("Test started: TC_2_30_1_1_Dashboard_DisplaysWelcomeMessage");

            // Step 1: Navigate to login page
            Console.WriteLine("Navigating to login page...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");
            Console.WriteLine("Navigation completed!");

            // Step 2: Enter valid login credentials
            Console.WriteLine("Entering valid login credentials...");

            // Find elements directly and fill them in
            var emailInput = driver.FindElement(By.Id("email"));
            emailInput.SendKeys("gebruiker@example.com");

            var passwordInput = driver.FindElement(By.Id("wachtwoord"));
            passwordInput.SendKeys("Wachtwoord123");

            Console.WriteLine("Login credentials entered.");

            // Step 3: Click login
            Console.WriteLine("Clicking login button...");
            var loginButton = driver.FindElement(By.Id("login-btn"));
            loginButton.Click();
            Console.WriteLine("Login submitted.");

            // Step 4: Wait until dashboard is loaded
            Console.WriteLine("Waiting for dashboard to appear...");
            wait.Until(d => d.FindElement(By.CssSelector("[data-test='welcome-message']")));
            Console.WriteLine("Dashboard successfully loaded!");

            // Step 5: Verify welcome message is visible
            Console.WriteLine("Verifying that the welcome message is visible...");

            var welcomeMessage = wait.Until(d =>
                d.FindElement(By.CssSelector("[data-test='welcome-message']")));

            Assert.IsTrue(welcomeMessage.Displayed,
                "Welcome message is not displayed on the dashboard.");

            // Additional verification
            string expectedName = "Test Gebruiker";
            Assert.IsTrue(welcomeMessage.Text.Contains(expectedName),
                $"Welcome message does not contain the expected name '{expectedName}'. Actual text: {welcomeMessage.Text}");

            Console.WriteLine($"Welcome message found: {welcomeMessage.Text}");

            Console.WriteLine("Test completed successfully.");
        }
    }
}
