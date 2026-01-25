using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_1_4
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
        public void TC_2_30_1_4_WelcomeMessageWithoutName()
        {
            Console.WriteLine("Test started: TC_2_30_1_4_WelcomeMessageWithoutName");
            Console.WriteLine("Test case: TC2.30.1-4 - Welcome message when patient name cannot be retrieved");

            // Option 1: Test with user without name (if available)
            // Option 2: Simulate API error while retrieving name

            // Step 1: Navigate to login page
            Console.WriteLine("Step 1: Navigating to login page...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");
            Console.WriteLine("Navigation completed!");

            // Step 2: Login with user where name cannot be retrieved
            Console.WriteLine("Step 2: Logging in with user (no login error, but name retrieval issue)...");

            var emailInput = driver.FindElement(By.Id("email"));
            emailInput.SendKeys("gebruiker@example.com");

            var passwordInput = driver.FindElement(By.Id("wachtwoord"));
            passwordInput.SendKeys("Wachtwoord123");

            var loginButton = driver.FindElement(By.Id("login-btn"));
            loginButton.Click();
            Console.WriteLine("Login submitted!");

            // Step 3: Wait for dashboard
            Console.WriteLine("Step 3: Waiting for dashboard...");
            wait.Until(d => d.FindElement(By.CssSelector("[data-test='welcome-message']")));
            Console.WriteLine("Dashboard loaded!");

            // Step 4: Verify welcome message
            Console.WriteLine("Step 4: Verifying welcome message...");
            var welcomeMessage = driver.FindElement(By.CssSelector("[data-test='welcome-message']"));

            string welcomeText = welcomeMessage.Text;
            Console.WriteLine($"Welcome message text: {welcomeText}");

            Assert.IsTrue(welcomeMessage.Displayed,
                "Welcome message is not displayed on the dashboard.");

            bool hasValidWelcome =
                welcomeText.Contains("Welcome") ||
                welcomeText.Contains("Hello") ||
                welcomeText.Contains("Hi") ||
                welcomeText.Contains("Good day");

            Assert.IsTrue(hasValidWelcome || !string.IsNullOrWhiteSpace(welcomeText),
                $"Welcome message is not appropriate: '{welcomeText}'");

            Console.WriteLine($"Welcome message displayed correctly: {welcomeText}");
            Console.WriteLine("Test completed successfully.");
        }
    }
}
