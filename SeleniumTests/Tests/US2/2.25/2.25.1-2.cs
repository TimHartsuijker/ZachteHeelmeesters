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
        public void TC_2_25_1_2_ProfilePage_OpensSuccessfully()
        {
            Console.WriteLine("Test gestart: TC_2_25_1_2_ProfilePage_OpensSuccessfully");

            // Stap 1: Ga naar loginpagina
            Console.WriteLine("Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");
            Console.WriteLine("Navigatie voltooid.");

            // Stap 2: Inloggegevens invoeren
            Console.WriteLine("Geldige inloggegevens invullen...");
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            Console.WriteLine("Inloggegevens ingevuld.");

            // Stap 3: Klik op Inloggen
            Console.WriteLine("Klikken op inloggen...");
            loginPage.ClickLogin();
            Console.WriteLine("Login verstuurd.");

            // Stap 4: Wachten op dashboard
            Console.WriteLine("Wachten tot dashboard geladen is...");
            wait.Until(d => d.FindElement(By.Id("dashboard-container")));
            Console.WriteLine("Dashboard succesvol geladen.");

            // Stap 5: Navigeren naar profielpagina
            Console.WriteLine("Klikken op 'Mijn profiel' in het navigatiemenu...");

            var profileButton = wait.Until(d =>
                d.FindElement(By.CssSelector("[data-test='nav-profile']"))
            );
            profileButton.Click();

            Console.WriteLine("'Mijn profiel' aangeklikt. Wachten tot profielpagina laadt...");

            // Stap 6: Controleren of profielpagina opent
            var profileContainer = wait.Until(d =>
                d.FindElement(By.Id("profile-page-container"))
            );

            Assert.IsTrue(profileContainer.Displayed,
                "De profielpagina is niet succesvol geopend.");

            Console.WriteLine("Profielpagina succesvol geladen.");
            Console.WriteLine("Test succesvol afgerond.");
        }
    }
}
