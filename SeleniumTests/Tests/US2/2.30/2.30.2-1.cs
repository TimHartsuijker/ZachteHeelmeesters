using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_2_1
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

            Console.WriteLine("Setup voltooid. Browser gestart.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            Console.WriteLine("Test afgerond. Browser wordt afgesloten.");
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void NavigationMenu_VisibleAfterLogin()
        {
            Console.WriteLine("Test gestart: NavigationMenu_VisibleAfterLogin");

            // Stap 1: Navigeren naar loginpagina
            Console.WriteLine("Stap 1: Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine("Navigatie voltooid!");

            // Stap 2: Geldige login invoeren
            Console.WriteLine("Stap 2: Email invoeren...");
            loginPage.EnterEmail("patient@example.com");
            Console.WriteLine("Stap 2b: Wachtwoord invoeren...");
            loginPage.EnterPassword("Test123!");
            Console.WriteLine("Stap 2c: Inloggen...");
            loginPage.ClickLogin();
            Console.WriteLine("Inloggen voltooid!");

            // Stap 3: Wachten tot dashboard geladen is
            Console.WriteLine("Stap 3: Wachten op dashboard...");
            wait.Until(d => d.FindElement(By.Id("dashboard-container")));
            Console.WriteLine("Dashboard geladen!");

            // Stap 4: Controleren of navigatiemenu zichtbaar is
            Console.WriteLine("Stap 4: Controleren navigatiemenu...");
            var navMenu = driver.FindElement(By.Id("navigation-menu"));
            Assert.IsTrue(navMenu.Displayed, "Het navigatiemenu is niet zichtbaar na het inloggen.");
            Console.WriteLine("Navigatiemenu is zichtbaar! Test geslaagd.");
        }
    }
}
