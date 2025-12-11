using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_25_2_4
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
        }

        [TestCleanup]
        public void Cleanup()
        {
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void TC_2_25_2_4_ProfileData_MatchesLoggedInPatient()
        {
            Console.WriteLine("Test gestart: TC_2_25_2_4_ProfileData_MatchesLoggedInPatient");

            // Stap 1: Login
            driver.Navigate().GoToUrl($"{baseUrl}/login");
            loginPage.EnterEmail("jan.jansen@example.com");
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();

            // Stap 2: Dashboard laden
            wait.Until(d => d.FindElement(By.Id("dashboard-container")));

            // Stap 3: Profielpagina openen
            wait.Until(d => d.FindElement(By.CssSelector("[data-test='nav-profile']"))).Click();

            // Stap 4: Vergelijken met verwachte data
            //Er kunnen nog meer hierbij om te controlleren indien nodig zoals adres, telefoonnummer etc.
            string expectedName = "Jan Jansen";
            string expectedEmail = "jan.jansen@example.com";

            string actualName = wait.Until(d => d.FindElement(By.Id("profile-name"))).Text.Trim();
            string actualEmail = wait.Until(d => d.FindElement(By.Id("profile-email"))).Text.Trim();

            Assert.AreEqual(expectedName, actualName, "Naam komt niet overeen met database.");
            Assert.AreEqual(expectedEmail, actualEmail, "E-mail komt niet overeen met database.");

            Console.WriteLine("Alle gegevens komen overeen met de ingelogde patiënt.");
        }
    }
}
