using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_25_1
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
        public void TC_2_25_1_1_ProfileButton_VisibleInNavigationMenu()
        {
            Console.WriteLine("Test gestart: TC_2_25_1_1_ProfileButton_VisibleInNavigationMenu");

            // Stap 1: Ga naar loginpagina
            Console.WriteLine("Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");
            Console.WriteLine("Navigatie voltooid.");

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
            Console.WriteLine("Dashboard succesvol geladen.");

            // Stap 5: Controleren of de profielknop zichtbaar is
            Console.WriteLine("Controleren of de knop 'Mijn profiel' zichtbaar is in het navigatiemenu...");

            var profileButton = wait.Until(d =>
                d.FindElement(By.CssSelector("[data-test='nav-profile']"))
            );

            Assert.IsTrue(profileButton.Displayed,
                "De knop naar de profielpagina is niet zichtbaar in het navigatiemenu.");

            Console.WriteLine("Knop 'Mijn profiel' succesvol gevonden.");
            Console.WriteLine($"Knop tekst: {profileButton.Text}");

            Console.WriteLine("Test succesvol afgerond.");
        }
    }
}
