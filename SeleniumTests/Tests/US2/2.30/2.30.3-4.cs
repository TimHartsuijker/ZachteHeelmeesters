using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_3_4
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
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void Dashboard_UpdatesAfterDataChange()
        {
            Console.WriteLine("Test gestart: Dashboard_UpdatesAfterDataChange");

            // Stap 1: Navigeren naar loginpagina en inloggen
            driver.Navigate().GoToUrl($"{baseUrl}/");
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();

            // Stap 2: Wachten tot dashboard summary zichtbaar is
            wait.Until(d => d.FindElement(By.Id("dashboard-summary")));

            // Stap 3: Backend update simuleren (Refresh)
            Console.WriteLine("Stap 3: Backend update simuleren...");
            driver.Navigate().Refresh();
            wait.Until(d => d.FindElement(By.Id("dashboard-summary")));

            // Stap 4: Controleren of updates zichtbaar zijn
            var appointments = driver.FindElement(By.Id("summary-next-appointment")).Text;
            Assert.IsTrue(appointments.Contains("NEW"), "Dashboard toont geen bijgewerkte gegevens.");

            Console.WriteLine("Dashboard toont updates correct. Test geslaagd!");
        }
    }
}
