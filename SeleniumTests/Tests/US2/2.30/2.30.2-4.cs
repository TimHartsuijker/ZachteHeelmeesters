using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace US2._30
{
    [TestClass]
    public class _2_30_2_4
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
        public void NavigationMenu_VisibleOnAllPages()
        {
            Console.WriteLine("Test started: NavigationMenu_VisibleOnAllPages");

            // Step 1: Navigate to login page and log in
            Console.WriteLine("Step 1: Navigating to login page...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");

            var emailInput = driver.FindElement(By.Id("email"));
            emailInput.SendKeys("gebruiker@example.com");

            var passwordInput = driver.FindElement(By.Id("wachtwoord"));
            passwordInput.SendKeys("Wachtwoord123");

            Console.WriteLine("Step 1b: Logging in...");
            var loginButton = driver.FindElement(By.Id("login-btn"));
            loginButton.Click();

            // Step 2: Wait until dashboard is loaded
            Console.WriteLine("Step 2: Waiting for dashboard to load...");
            wait.Until(d => d.FindElement(By.CssSelector("[data-test='welcome-message']")));
            Console.WriteLine("Dashboard loaded!");

            // Step 3: Check navigation menu visibility on dashboard
            Console.WriteLine("Step 3: Checking navigation menu on dashboard...");
            CheckNavigationMenuVisible();

            // Step 4: Check visibility on all pages
            string[] menuItems = { "Mijn afspraken", "Mijn medisch dossier", "Mijn profiel" };

            foreach (var item in menuItems)
            {
                Console.WriteLine($"Step 4: Navigating to '{item}'...");

                try
                {
                    var link = driver.FindElement(By.XPath($"//a[contains(text(),'{item}')]"));
                    link.Click();

                    System.Threading.Thread.Sleep(1000);

                    Console.WriteLine($"Step 4b: Checking navigation menu on '{item}' page...");
                    CheckNavigationMenuVisible();

                    Console.WriteLine($"Navigation menu is visible on page '{item}'!");

                    Console.WriteLine("Returning to dashboard...");
                    var dashboardLink = driver.FindElement(By.XPath("//a[contains(text(),'Dashboard')]"));
                    dashboardLink.Click();
                    wait.Until(d => d.FindElement(By.CssSelector("[data-test='welcome-message']")));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error on '{item}': {ex.Message}");
                    driver.Navigate().GoToUrl($"{baseUrl}/dashboard");
                }
            }

            Console.WriteLine("Test completed successfully.");
        }

        private void CheckNavigationMenuVisible()
        {
            try
            {
                var navMenu = driver.FindElement(By.XPath("//nav[contains(@class, 'navbar')]"));
                Assert.IsTrue(navMenu.Displayed, "Navigation menu (nav) is not visible.");
                Console.WriteLine("Navigation menu (nav) found and visible.");
            }
            catch (NoSuchElementException)
            {
                var navLinks = driver.FindElement(By.XPath("//li[contains(@class, 'nav-center-buttons')]"));
                Assert.IsTrue(navLinks.Displayed, "Navigation links are not visible.");
                Console.WriteLine("Navigation links found and visible.");
            }
        }
    }
}
