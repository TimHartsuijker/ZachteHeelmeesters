using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_2_4
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
        public void NavigationMenu_VisibleOnAllPages()
        {
            Console.WriteLine("Test gestart: NavigationMenu_VisibleOnAllPages");

            // Stap 1: Navigeren naar loginpagina en inloggen
            Console.WriteLine("Stap 1: Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/");
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            Console.WriteLine("Stap 1b: Inloggen...");
            loginPage.ClickLogin();

            // Stap 2: Wachten tot navigatiemenu geladen is
            Console.WriteLine("Stap 2: Wachten tot navigatiemenu zichtbaar is...");
            wait.Until(d => d.FindElement(By.Id("navigation-menu")));

            // Stap 3: Controleren zichtbaarheid op alle pagina's
            string[] menuItems = { "Dashboard", "Afspraken", "Facturen", "Medisch dossier" };
            foreach (var item in menuItems)
            {
                Console.WriteLine($"Stap 3: Navigeren naar '{item}'...");
                driver.FindElement(By.XPath($"//*[contains(text(),'{item}')]")).Click();

                Console.WriteLine("Stap 3b: Controleren of navigatiemenu zichtbaar blijft...");
                var navMenu = driver.FindElement(By.Id("navigation-menu"));
                Assert.IsTrue(navMenu.Displayed, $"Navigatiemenu is verdwenen op de pagina: {item}");
                Console.WriteLine($"Navigatiemenu is zichtbaar op pagina '{item}'!");
            }

            Console.WriteLine("Test succesvol afgerond.");
        }
    }
}
