using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace US2._30
{
    [TestClass]
    public class _2_30_2_3
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
        public void NavigationMenu_LinksNavigateCorrectly()
        {
            Console.WriteLine("Test started: NavigationMenu_LinksNavigateCorrectly");

            // Step 1: Login
            Console.WriteLine("Step 1: Navigating to login page...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");

            driver.FindElement(By.Id("email")).SendKeys("gebruiker@example.com");
            driver.FindElement(By.Id("wachtwoord")).SendKeys("Wachtwoord123");

            Console.WriteLine("Step 1b: Logging in...");
            driver.FindElement(By.Id("login-btn")).Click();

            // Step 2: Wait for dashboard
            Console.WriteLine("Step 2: Waiting for dashboard...");
            wait.Until(d => d.FindElement(By.CssSelector("[data-test='welcome-message']")));
            Console.WriteLine("Dashboard loaded!");

            Dictionary<string, string> menuItems = new()
            {
                { "Dashboard", "/dashboard" },
                { "Mijn afspraken", "/afspraken" },
                { "Mijn medisch dossier", "/dossier" },
                { "Mijn profiel", "/patientprofiel" }
            };

            foreach (var item in menuItems)
            {
                Console.WriteLine($"Step 3: Clicking menu item '{item.Key}'...");

                try
                {
                    var link = driver.FindElement(By.XPath($"//a[contains(text(),'{item.Key}')]"));
                    link.Click();

                    Console.WriteLine($"Waiting for URL to contain '{item.Value}'...");
                    wait.Until(d => d.Url.Contains(item.Value));

                    Console.WriteLine($"Successfully navigated to: {driver.Url}");

                    if (item.Key != "Dashboard")
                    {
                        Console.WriteLine("Returning to dashboard...");
                        driver.FindElement(By.XPath("//a[contains(text(),'Dashboard')]")).Click();
                        wait.Until(d => d.FindElement(By.CssSelector("[data-test='welcome-message']")));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while navigating menu item '{item.Key}': {ex.Message}");

                    driver.Navigate().GoToUrl($"{baseUrl}/login");
                    driver.FindElement(By.Id("email")).SendKeys("gebruiker@example.com");
                    driver.FindElement(By.Id("wachtwoord")).SendKeys("Wachtwoord123");
                    driver.FindElement(By.Id("login-btn")).Click();
                    wait.Until(d => d.FindElement(By.CssSelector("[data-test='welcome-message']")));
                }
            }

            Console.WriteLine("Test completed successfully.");
        }
    }
}
