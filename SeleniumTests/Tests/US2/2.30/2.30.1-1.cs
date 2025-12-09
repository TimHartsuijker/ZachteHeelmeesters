using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_1
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
        public void TC_2_30_1_1_Dashboard_DisplaysWelcomeMessage()
        {
            Console.WriteLine("Test gestart: TC_2_30_1_1_Dashboard_DisplaysWelcomeMessage");

            // Stap 1: Ga naar loginpagina
            Console.WriteLine("Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");
            Console.WriteLine("Navigatie voltooid!");

            // Stap 2: Geldige inloggegevens invoeren
            Console.WriteLine("Geldige inloggegevens invullen...");
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            Console.WriteLine("Inloggegevens ingevuld.");

            // Stap 3: Klik op Inloggen
            Console.WriteLine("Klikken op inloggen...");
            loginPage.ClickLogin();
            Console.WriteLine("Login verstuurd.");

            // Stap 4: Wachten tot dashboard geladen is
            Console.WriteLine("Wachten tot dashboard verschijnt...");
            wait.Until(d => d.FindElement(By.Id("dashboard-container")));
            Console.WriteLine("Dashboard succesvol geladen!");

            // Stap 5: Check of welkomstboodschap zichtbaar is
            Console.WriteLine("Controleren of de welkomstboodschap zichtbaar is...");
            var welcomeMessage = driver.FindElement(By.XPath("//*[contains(text(),'Welkom')]"));

            Assert.IsTrue(welcomeMessage.Displayed,
                "Welkomstboodschap wordt niet weergegeven op het dashboard.");
            Console.WriteLine("Welkomstboodschap succesvol gevonden!");

            Console.WriteLine("Test succesvol afgerond.");
        }
    }
}
