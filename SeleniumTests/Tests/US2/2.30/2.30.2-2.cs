using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_2_2
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
            Console.WriteLine("Setup completed.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            Console.WriteLine("Test finished. Closing browser.");
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void NavigationMenu_ContainsCorrectOptions()
        {
            Console.WriteLine("Test started: NavigationMenu_ContainsCorrectOptions");
            Console.WriteLine("Test case: TC2.30.2-2 - Verifying available dashboard functionality");
            Console.WriteLine("NOTE: The dashboard uses panels instead of a traditional menu.");

            // Step 1: Login
            Console.WriteLine("Step 1: Navigating to login page...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");

            Console.WriteLine("Step 1b: Entering login credentials...");
            driver.FindElement(By.Id("email")).SendKeys("gebruiker@example.com");
            driver.FindElement(By.Id("wachtwoord")).SendKeys("Wachtwoord123");

            Console.WriteLine("Step 1c: Logging in...");
            driver.FindElement(By.Id("login-btn")).Click();

            // Step 2: Wait for dashboard
            Console.WriteLine("Step 2: Waiting for dashboard...");
            wait.Until(d => d.FindElement(By.CssSelector("[data-test='welcome-message']")));
            Console.WriteLine("Dashboard loaded!");

            // Step 3: Verify functionality
            Console.WriteLine("Step 3: Verifying available functionality...");

            var welcomeMessage = driver.FindElement(By.CssSelector("[data-test='welcome-message']"));
            Assert.IsTrue(welcomeMessage.Displayed, "Welcome message is not visible.");
            Console.WriteLine($"✓ Welcome message: {welcomeMessage.Text}");

            var leftPanel = driver.FindElement(By.ClassName("panel-left"));
            var rightPanel = driver.FindElement(By.ClassName("panel-right"));

            Assert.IsTrue(leftPanel.Displayed, "Left panel is not visible.");
            Assert.IsTrue(rightPanel.Displayed, "Right panel is not visible.");

            var leftTitle = leftPanel.FindElement(By.TagName("h2")).Text;
            var rightTitle = rightPanel.FindElement(By.TagName("h2")).Text;

            Console.WriteLine($"✓ Left panel title: {leftTitle}");
            Console.WriteLine($"✓ Right panel title: {rightTitle}");

            bool hasAppointments = leftTitle.Contains("Afspraken") || leftTitle.Contains("Appointments");
            bool hasReferrals = rightTitle.Contains("Doorverwijzingen") || rightTitle.Contains("Referrals");

            Assert.IsTrue(hasAppointments, $"Left panel does not contain appointments. Found: {leftTitle}");
            Assert.IsTrue(hasReferrals, $"Right panel does not contain referrals. Found: {rightTitle}");

            Console.WriteLine("✓ Test passed: Dashboard shows correct functionality.");
        }
    }
}
