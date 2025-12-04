using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _1_19_5
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

        // ------------------------
        // 1?? Goed e-mail + fout wachtwoord
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_CorrectEmail_WrongPassword()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/");
            loginPage.EnterEmail("gebruiker@example.com");
            loginPage.EnterPassword("FoutWw123");
            loginPage.ClickLogin();

            var error = wait.Until(d => d.FindElement(By.Id("login-error")));
            Assert.AreEqual("inloggegevens zijn incorrect", error.Text);
        }

        // ------------------------
        // 2?? Fout e-mail + goed wachtwoord
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_WrongEmail_CorrectPassword()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/");
            loginPage.EnterEmail("gebruikerexample.com");
            loginPage.EnterPassword("Wachtwoord123");
            loginPage.ClickLogin();

            var error = wait.Until(d => d.FindElement(By.Id("login-error")));
            Assert.AreEqual("inloggegevens zijn incorrect", error.Text);
        }

        // ------------------------
        // 3?? Beide verkeerd
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_WrongEmail_WrongPassword()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/");
            loginPage.EnterEmail("gebruikerexample.com");
            loginPage.EnterPassword("Wachtwoord");
            loginPage.ClickLogin();

            var error = wait.Until(d => d.FindElement(By.Id("login-error")));
            Assert.AreEqual("inloggegevens zijn incorrect", error.Text);
        }

        // ------------------------
        // 4?? E-mail leeg + wachtwoord ingevuld
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_EmptyEmail_CorrectPassword()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/");
            loginPage.EnterEmail("");
            loginPage.EnterPassword("Wachtwoord123");
            loginPage.ClickLogin();

            var error = wait.Until(d => d.FindElement(By.Id("empty-input-error")));
            Assert.AreEqual("gegevens moeten ingevuld zijn", error.Text);
        }

        // ------------------------
        // 5?? E-mail ingevuld + wachtwoord leeg
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_CorrectEmail_EmptyPassword()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/");
            loginPage.EnterEmail("gebruiker@example.com");
            loginPage.EnterPassword("");
            loginPage.ClickLogin();

            var error = wait.Until(d => d.FindElement(By.Id("empty-input-error")));
            Assert.AreEqual("gegevens moeten ingevuld zijn", error.Text);
        }

        // ------------------------
        // 6?? Beide leeg
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_BothFieldsEmpty()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/");
            loginPage.EnterEmail("");
            loginPage.EnterPassword("");
            loginPage.ClickLogin();

            var error = wait.Until(d => d.FindElement(By.Id("empty-input-error")));
            Assert.AreEqual("gegevens moeten ingevuld zijn", error.Text);
        }
    }
}
