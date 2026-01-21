using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _1_19_4_1
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "http://localhost";
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
        public void TC_1_19_4_CorrecteLoginNaarPortaal()
        {
            Console.WriteLine("Test gestart: TC_1_19_4_CorrecteLoginNaarPortaal");

            
            Console.WriteLine("Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine("Loginpagina geladen.");

            
            Console.WriteLine("E-mailadres invoeren: gebruiker@example.com");
            loginPage.EnterEmail("gebruiker@example.com");

            Console.WriteLine("Wachtwoord invoeren: Wachtwoord123");
            loginPage.EnterPassword("Wachtwoord123");

            
            Console.WriteLine("Op inlogknop klikken...");
            loginPage.ClickLogin();

            
            Console.WriteLine("Wachten tot gebruiker wordt doorgestuurd naar /dashboard...");
            wait.Until(d => d.Url.Contains("/dashboard"));
            Console.WriteLine("Redirect naar dashboard gedetecteerd!");

            
            Console.WriteLine("Controleren of gebruiker op het dashboard terecht is gekomen...");
            Assert.IsTrue(driver.Url.Contains("/dashboard"),
                "Gebruiker is niet doorgestuurd naar het dashboard.");

            Console.WriteLine("? Succesvolle login! Gebruiker is op het dashboard.");
            Console.WriteLine("Test succesvol afgerond.");
        }

    }
}
