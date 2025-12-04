using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _1_19_4_1
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
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void TC_1_19_4_CorrecteLoginNaarPortaal()
        {
            // Navigeer naar loginpagina
            driver.Navigate().GoToUrl($"{baseUrl}/");

            
            loginPage.EnterEmail("gebruiker@example.com");
            loginPage.EnterPassword("Wachtwoord123");
            loginPage.ClickLogin();

            
            wait.Until(d => d.Url.Contains("/dashboard"));

            
            Assert.IsTrue(driver.Url.Contains("/dashboard"), "Gebruiker is niet doorgestuurd naar het dashboard.");
        }
    }
}
