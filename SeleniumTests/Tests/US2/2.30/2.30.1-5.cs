using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_1_5
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
        public void TC_2_30_1_5_Dashboard_NameMatchesDatabase()
        {
            Console.WriteLine("Test started: TC_2_30_1_5_Dashboard_NameMatchesDatabase");

            string expectedName = "Test Gebruiker"; // Name of gebruiker@example.com in database

            // Step 1: Navigate to login page
            Console.WriteLine("Step 1: Navigating to login page...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");
            Console.WriteLine("Navigation completed!");

            // Step 2: Enter login credentials
            Console.WriteLine("Step 2: Entering login credentials...");
            var emailInput = driver.FindElement(By.Id("email"));
            emailInput.SendKeys("gebruiker@example.com");

            var passwordInput = driver.FindElement(By.Id("wachtwoord"));
            passwordInput.SendKeys("Wachtwoord123");
            Console.WriteLine("Login credentials entered!");

            // Step 3: Click login
            Console.WriteLine("Step 3: Clicking login...");
            var loginButton = driver.FindElement(By.Id("login-btn"));
            loginButton.Click();
            Console.WriteLine("Login submitted!");

            // Step 4: Wait for dashboard
            Console.WriteLine("Step 4: Loading dashboard...");
            wait.Until(d => d.FindElement(By.CssSelector("[data-test='welcome-message']")));
            Console.WriteLine("Dashboard successfully loaded!");

            // Step 5: Verify welcome message name
            Console.WriteLine("Step 5: Verifying name in welcome message...");
            var message = driver.FindElement(By.CssSelector("[data-test='welcome-message']"));

            Assert.IsTrue(message.Text.Contains(expectedName),
                $"Name does not match. Expected: {expectedName} — Actual: {message.Text}");

            Console.WriteLine($"PASS: Welcome name matches database value: {message.Text}");
            Console.WriteLine("Test completed successfully.");
        }
    }
}
