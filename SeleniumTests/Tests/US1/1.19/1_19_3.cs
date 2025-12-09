using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _1_19_3
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "http://localhost:5173";
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
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void TC_1_19_3_ValidatieCorrecteInlog()
        {
            Console.WriteLine("Test gestart: TC_1_19_3_ValidatieCorrecteInlog");

            
            Console.WriteLine("Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine("Loginpagina geladen.");

           
            Console.WriteLine("E-mailadres invoeren: gebruiker@example.com");
            loginPage.EnterEmail("gebruiker@example.com");

            Console.WriteLine("Wachtwoord invoeren: Wachtwoord123");
            loginPage.EnterPassword("Wachtwoord123");

            Console.WriteLine("Op inlogknop klikken...");
            loginPage.ClickLogin();

            
            Console.WriteLine("Wachten op redirect naar dashboard...");
            wait.Until(d => d.Url.Contains("/dashboard") || !d.Url.EndsWith("/login"));
            Console.WriteLine("Redirect gedetecteerd!");

            
            Console.WriteLine("Controleren of gebruiker NIET op loginpagina is gebleven...");
            Assert.IsFalse(driver.Url.Contains("login"),
                "Gebruiker bleef op loginpagina na correcte inlog.");

            Console.WriteLine("? Succesvolle loginvalidatie uitgevoerd. Gebruiker is doorgestuurd.");
            Console.WriteLine("Test succesvol afgerond.");
        }
    }
}
