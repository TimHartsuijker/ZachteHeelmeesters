using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_3_3
    {
        private IWebDriver driver;
        private string baseUrl = "https://localhost:5173";
        private LoginPage loginPage;

        [TestInitialize]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            driver = new ChromeDriver(options);
            loginPage = new LoginPage(driver);
            Console.WriteLine("Setup voltooid.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void Dashboard_OverviewReadable()
        {
            Console.WriteLine("Test gestart: Dashboard_OverviewReadable");

            // Stap 1: Navigeren naar loginpagina en inloggen
            Console.WriteLine("Stap 1: Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/");
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            Console.WriteLine("Stap 1b: Inloggen...");
            loginPage.ClickLogin();

            // Stap 2: Controleren structuur
            driver.FindElement(By.Id("dashboard-summary"));
            bool hasSections = driver.FindElements(By.ClassName("summary-section")).Count >= 1;
            Assert.IsTrue(hasSections, "Dashboard mist structuur of secties.");

            Console.WriteLine("Dashboard-overzicht is gestructureerd en leesbaar. Test geslaagd!");
        }
    }
}
