using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_3_1
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
            Console.WriteLine("Test afgerond. Browser wordt afgesloten.");
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void Dashboard_CompactSummaryVisible()
        {
            Console.WriteLine("Test started: Dashboard_CompactSummaryVisible");

            // Step 1: Navigate to login page
            Console.WriteLine("Step 1: Navigating to login page...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");

            // Step 2: Log in
            Console.WriteLine("Step 2: Entering login credentials...");
            driver.FindElement(By.Id("email")).SendKeys("gebruiker@example.com");
            driver.FindElement(By.Id("wachtwoord")).SendKeys("Wachtwoord123");

            Console.WriteLine("Step 2b: Logging in...");
            driver.FindElement(By.Id("login-btn")).Click();

            // Step 3: Wait for dashboard summary
            Console.WriteLine("Step 3: Waiting for dashboard summary...");
            wait.Until(d => d.FindElement(By.ClassName("dashboard-grid")));
            wait.Until(d => d.FindElement(By.ClassName("panel-left")));
            wait.Until(d => d.FindElement(By.ClassName("panel-right")));

            // Step 4: Verify summary visibility
            Console.WriteLine("Step 4: Verifying dashboard summary...");

            var welcomeMessage = driver.FindElement(By.CssSelector("[data-test='welcome-message']"));
            Assert.IsTrue(welcomeMessage.Displayed, "Welcome message is not visible.");
            Console.WriteLine("Welcome message is visible!");

            var dashboardGrid = driver.FindElement(By.ClassName("dashboard-grid"));
            Assert.IsTrue(dashboardGrid.Displayed, "Dashboard grid is not visible.");
            Console.WriteLine("Dashboard grid is visible!");

            var leftPanel = driver.FindElement(By.ClassName("panel-left"));
            Assert.IsTrue(leftPanel.Displayed, "Left panel (Appointments) is not visible.");

            var rightPanel = driver.FindElement(By.ClassName("panel-right"));
            Assert.IsTrue(rightPanel.Displayed, "Right panel (Referrals) is not visible.");

            var panels = driver.FindElements(By.CssSelector(".dashboard-grid > div"));
            Assert.IsTrue(panels.Count >= 2, "Dashboard has fewer than 2 panels.");

            Console.WriteLine("Dashboard compact summary is fully visible. Test passed.");
        }
    }
}