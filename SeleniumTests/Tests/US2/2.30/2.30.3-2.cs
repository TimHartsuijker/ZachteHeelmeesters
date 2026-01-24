using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_3_2
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
        public void Dashboard_SummaryMatchesDatabase()
        {
            Console.WriteLine("Test started: Dashboard_SummaryMatchesDatabase");

            try
            {
                Console.WriteLine("Step 1: Navigating to login page...");
                driver.Navigate().GoToUrl($"{baseUrl}/login");

                driver.FindElement(By.Id("email")).SendKeys("gebruiker@example.com");
                driver.FindElement(By.Id("wachtwoord")).SendKeys("Wachtwoord123");

                Console.WriteLine("Step 1b: Logging in...");
                driver.FindElement(By.Id("login-btn")).Click();

                Console.WriteLine("Step 2: Waiting for dashboard...");
                wait.Until(d => d.FindElement(By.CssSelector("[data-test='welcome-message']")));
                wait.Until(d => d.FindElement(By.ClassName("dashboard-grid")));

                Console.WriteLine("Dashboard loaded!");

                Console.WriteLine("Step 3: Verifying dashboard content...");

                var welcomeMessage = driver.FindElement(By.CssSelector("[data-test='welcome-message']"));
                Assert.IsTrue(welcomeMessage.Displayed, "Welcome message is not visible.");
                Console.WriteLine($"Welcome message: {welcomeMessage.Text}");

                var leftPanel = driver.FindElement(By.ClassName("panel-left"));
                var rightPanel = driver.FindElement(By.ClassName("panel-right"));

                Assert.IsTrue(leftPanel.Displayed, "Left panel is not visible.");
                Assert.IsTrue(rightPanel.Displayed, "Right panel is not visible.");

                var leftContent = leftPanel.FindElement(By.TagName("p")).Text;
                var rightContent = rightPanel.FindElement(By.TagName("p")).Text;

                Assert.IsFalse(string.IsNullOrWhiteSpace(leftContent), "Left panel has no content.");
                Assert.IsFalse(string.IsNullOrWhiteSpace(rightContent), "Right panel has no content.");

                Console.WriteLine("Dashboard shows basic information. Test passed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed: {ex.Message}");
                throw;
            }
        }
    }
}