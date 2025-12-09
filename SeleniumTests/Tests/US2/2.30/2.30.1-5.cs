using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_1_5
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
        public void TC_2_30_1_5_Dashboard_NameMatchesDatabase()
        {
            Console.WriteLine("Test gestart: TC_2_30_1_5_Dashboard_NameMatchesDatabase");

            string expectedName = "John Doe"; // aanpassen aan database

            // Stap 1: Navigeren naar loginpagina
            Console.WriteLine("Stap 1: Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");
            Console.WriteLine("Navigatie voltooid!");

            // Stap 2: Inloggegevens invoeren
            Console.WriteLine("Stap 2: Inloggegevens invullen...");
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            Console.WriteLine("Inloggegevens ingevuld!");

            // Stap 3: Klik op Inloggen
            Console.WriteLine("Stap 3: Klikken op inloggen...");
            loginPage.ClickLogin();
            Console.WriteLine("Login verstuurd!");

            // Stap 4: Wachten tot dashboard geladen is
            Console.WriteLine("Stap 4: Dashboard laden...");
            wait.Until(d => d.FindElement(By.Id("dashboard-container")));
            Console.WriteLine("Dashboard succesvol geladen!");

            // Stap 5: Ophalen en controleren welkomsttekst
            Console.WriteLine("Stap 5: Controleren naam in welkomstboodschap...");
            var message = driver.FindElement(By.XPath("//*[contains(text(),'Welkom')]"));

            Assert.IsTrue(message.Text.Contains(expectedName),
                $"Naam komt niet overeen. Verwacht: {expectedName} — Kreeg: {message.Text}");

            Console.WriteLine("PASS: Welkomstnaam komt overeen met database.");
            Console.WriteLine("Test succesvol afgerond.");
        }
    }
}
