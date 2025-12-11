using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_3_1
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "https://localhost:5173";
        private LoginPage loginPage;

        [TestInitialize]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            loginPage = new LoginPage(driver);
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
            Console.WriteLine("Test gestart: Dashboard_CompactSummaryVisible");

            // Stap 1: Navigeren naar loginpagina
            Console.WriteLine("Stap 1: Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/");

            // Stap 2: Inloggen
            Console.WriteLine("Stap 2: Inloggegevens invoeren...");
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            Console.WriteLine("Stap 2b: Inloggen...");
            loginPage.ClickLogin();

            // Stap 3: Wachten tot dashboard summary zichtbaar is
            Console.WriteLine("Stap 3: Wachten op dashboard summary...");
            wait.Until(d => d.FindElement(By.Id("dashboard-summary")));

            // Stap 4: Controleren zichtbaarheid
            var summary = driver.FindElement(By.Id("dashboard-summary"));
            Assert.IsTrue(summary.Displayed, "Compact summary is niet zichtbaar.");
            Console.WriteLine("Dashboard summary is zichtbaar! Test geslaagd.");
        }
    }
}
