using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_2_1
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
            Console.WriteLine("Setup completed. Browser started.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            Console.WriteLine("Test finished. Closing browser.");
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void NavigationMenu_VisibleAfterLogin()
        {
            Console.WriteLine("Test started: NavigationMenu_VisibleAfterLogin");
            Console.WriteLine("Test case: TC2.30.2-1 - Visibility of navigation elements after login");
            Console.WriteLine("NOTE: The dashboard does not have a traditional menu, but panels representing functionality.");

            // Step 1: Navigate to login page
            Console.WriteLine("Step 1: Navigating to login page...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");
            Console.WriteLine("Navigation completed!");

            // Step 2: Enter valid login credentials
            Console.WriteLine("Step 2: Entering email...");
            var emailInput = driver.FindElement(By.Id("email"));
            emailInput.SendKeys("gebruiker@example.com");

            Console.WriteLine("Step 2b: Entering password...");
            var passwordInput = driver.FindElement(By.Id("wachtwoord"));
            passwordInput.SendKeys("Wachtwoord123");

            Console.WriteLine("Step 2c: Logging in...");
            var loginButton = driver.FindElement(By.Id("login-btn"));
            loginButton.Click();
            Console.WriteLine("Login completed!");

            // Step 3: Wait for dashboard
            Console.WriteLine("Step 3: Waiting for dashboard...");
            wait.Until(d => d.FindElement(By.CssSelector("[data-test='welcome-message']")));
            Console.WriteLine("Dashboard loaded!");

            // Step 4: Verify dashboard navigation elements
            Console.WriteLine("Step 4: Verifying dashboard navigation elements...");

            var welcomeMessage = driver.FindElement(By.CssSelector("[data-test='welcome-message']"));
            Assert.IsTrue(welcomeMessage.Displayed, "Welcome message is not visible.");

            var dashboardGrid = driver.FindElement(By.ClassName("dashboard-grid"));
            Assert.IsTrue(dashboardGrid.Displayed, "Dashboard grid is not visible.");

            var leftPanel = driver.FindElement(By.ClassName("panel-left"));
            Assert.IsTrue(leftPanel.Displayed, "Left panel (Appointments) is not visible.");

            var rightPanel = driver.FindElement(By.ClassName("panel-right"));
            Assert.IsTrue(rightPanel.Displayed, "Right panel (Referrals) is not visible.");

            Console.WriteLine("✓ Dashboard navigation elements are visible:");
            Console.WriteLine($"  - Welcome message: {welcomeMessage.Text}");
            Console.WriteLine($"  - Left panel: {leftPanel.FindElement(By.TagName("h2")).Text}");
            Console.WriteLine($"  - Right panel: {rightPanel.FindElement(By.TagName("h2")).Text}");

            Console.WriteLine("Test passed: Dashboard displays all navigation elements.");
        }
    }
}
