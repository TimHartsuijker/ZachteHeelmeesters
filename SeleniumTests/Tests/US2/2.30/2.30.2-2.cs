using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_2_2
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
        public void NavigationMenu_ContainsCorrectOptions()
        {
            Console.WriteLine("Test gestart: NavigationMenu_ContainsCorrectOptions");

            // Stap 1: Navigeren naar loginpagina en inloggen
            Console.WriteLine("Stap 1: Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine("Stap 1b: Inloggegevens invoeren...");
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            Console.WriteLine("Stap 1c: Inloggen...");
            loginPage.ClickLogin();

            // Stap 2: Wachten tot dashboard geladen is
            Console.WriteLine("Stap 2: Wachten tot dashboard zichtbaar is...");
            wait.Until(d => d.FindElement(By.Id("dashboard-container")));
            Console.WriteLine("Dashboard geladen!");

            // Stap 3: Controleren menu-opties
            string[] expectedItems = { "Dashboard", "Afspraken", "Facturen", "Medisch dossier" };
            Console.WriteLine("Stap 3: Controleren navigatiemenu zichtbaar...");
            var navMenu = driver.FindElement(By.Id("navigation-menu"));
            Assert.IsTrue(navMenu.Displayed, "Navigatiemenu is niet zichtbaar.");

            foreach (string item in expectedItems)
            {
                Console.WriteLine($"Zoeken naar menu item: {item}");
                var element = navMenu.FindElement(By.XPath($".//*[contains(text(),'{item}')]"));
                Assert.IsTrue(element.Displayed, $"Menu item '{item}' is niet zichtbaar.");
                Console.WriteLine($"Menu item '{item}' gevonden!");
            }

            Console.WriteLine("Test succesvol afgerond.");
        }
    }
}
