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
        private string baseUrl = "https://localhost:7058";
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
            // 1. Navigeer naar loginpagina
            driver.Navigate().GoToUrl($"{baseUrl}/");

            // 2. Vul correcte login in
            loginPage.EnterEmail("gebruiker@example.com");
            loginPage.EnterPassword("Wachtwoord123");
            loginPage.ClickLogin();

            // 3. Wacht tot gebruiker doorgestuurd is
            wait.Until(d => d.Url.Contains("/dashboard") || !d.Url.EndsWith("/login"));

            // 4. Controleer dat gebruiker niet op loginpagina blijft
            Assert.IsFalse(driver.Url.Contains("login"), "Gebruiker bleef op loginpagina na correcte inlog.");
        }
    }
}
