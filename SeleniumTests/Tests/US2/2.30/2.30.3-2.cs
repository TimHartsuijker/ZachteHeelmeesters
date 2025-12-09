using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_3_2
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
        public void Dashboard_SummaryMatchesDatabase()
        {
            Console.WriteLine("Test gestart: Dashboard_SummaryMatchesDatabase");

            // Verwachte database-waarden
            string expectedNextAppointment = "12-12-2025";
            string expectedUnreadMessages = "3";

            // Stap 1: Navigeren naar loginpagina en inloggen
            Console.WriteLine("Stap 1: Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/");
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            Console.WriteLine("Stap 1b: Inloggen...");
            loginPage.ClickLogin();

            // Stap 2: Wachten tot dashboard summary zichtbaar is
            Console.WriteLine("Stap 2: Wachten op dashboard summary...");
            wait.Until(d => d.FindElement(By.Id("dashboard-summary")));

            // Stap 3: Gegevens controleren
            var nextAppt = driver.FindElement(By.Id("summary-next-appointment")).Text;
            var unreadMsg = driver.FindElement(By.Id("summary-unread-messages")).Text;

            Assert.IsTrue(nextAppt.Contains(expectedNextAppointment),
                "Volgende afspraak komt niet overeen met database.");
            Assert.IsTrue(unreadMsg.Contains(expectedUnreadMessages),
                "Aantal ongelezen berichten komt niet overeen met database.");

            Console.WriteLine("Summary-gegevens komen overeen met database. Test geslaagd!");
        }
    }
}
