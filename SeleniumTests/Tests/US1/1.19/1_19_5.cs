using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _1_19_5
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

        // ------------------------
        // 1?? Goed e-mail + fout wachtwoord
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_CorrectEmail_WrongPassword()
        {
            Console.WriteLine("Test: CorrectEmail_WrongPassword");

            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine("Loginpagina geladen.");

            loginPage.EnterEmail("gebruiker@example.com");
            Console.WriteLine("Correct e-mailadres ingevoerd.");

            loginPage.EnterPassword("FoutWw123");
            Console.WriteLine("Fout wachtwoord ingevoerd.");

            loginPage.ClickLogin();
            Console.WriteLine("Loginpoging uitgevoerd.");

            var error = wait.Until(d => d.FindElement(By.Id("login-error")));
            Console.WriteLine("Foutmelding gevonden: " + error.Text);

            Assert.AreEqual("Inloggegevens zijn incorrect", error.Text);
            Console.WriteLine(" Test geslaagd.\n");
        }

        // ------------------------
        // 2?? Fout e-mail + goed wachtwoord
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_WrongEmail_CorrectPassword()
        {
            Console.WriteLine("Test: WrongEmail_CorrectPassword");

            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine("Loginpagina geladen.");

            loginPage.EnterEmail("gebruikerexample.com");
            Console.WriteLine("Fout e-mailadres ingevoerd.");

            loginPage.EnterPassword("Wachtwoord123");
            Console.WriteLine("Correct wachtwoord ingevoerd.");

            loginPage.ClickLogin();
            Console.WriteLine("Loginpoging uitgevoerd.");

            var error = wait.Until(d => d.FindElement(By.Id("login-error")));
            Console.WriteLine("Foutmelding gevonden: " + error.Text);

            Assert.AreEqual("Inloggegevens zijn incorrect", error.Text);
            Console.WriteLine(" Test geslaagd.\n");
        }

        // ------------------------
        // 3?? Beide verkeerd
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_WrongEmail_WrongPassword()
        {
            Console.WriteLine("Test: WrongEmail_WrongPassword");

            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine("Loginpagina geladen.");

            loginPage.EnterEmail("gebruikerexample.com");
            Console.WriteLine("Fout e-mailadres ingevoerd.");

            loginPage.EnterPassword("Wachtwoord");
            Console.WriteLine("Fout wachtwoord ingevoerd.");

            loginPage.ClickLogin();
            Console.WriteLine("Loginpoging uitgevoerd.");

            var error = wait.Until(d => d.FindElement(By.Id("login-error")));
            Console.WriteLine("Foutmelding gevonden: " + error.Text);

            Assert.AreEqual("Inloggegevens zijn incorrect", error.Text);
            Console.WriteLine(" Test geslaagd.\n");
        }

        // ------------------------
        // 4?? E-mail leeg + wachtwoord ingevuld
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_EmptyEmail_CorrectPassword()
        {
            Console.WriteLine("Test: EmptyEmail_CorrectPassword");

            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine("Loginpagina geladen.");

            loginPage.EnterEmail("");
            Console.WriteLine("Leeg e-mailadres ingevoerd.");

            loginPage.EnterPassword("Wachtwoord123");
            Console.WriteLine("Correct wachtwoord ingevoerd.");

            loginPage.ClickLogin();
            Console.WriteLine("Loginpoging uitgevoerd.");

            var error = wait.Until(d => d.FindElement(By.Id("empty-input-error")));
            Console.WriteLine("Foutmelding gevonden: " + error.Text);

            Assert.AreEqual("Gegevens moeten ingevuld zijn", error.Text);
            Console.WriteLine(" Test geslaagd.\n");
        }

        // ------------------------
        // 5?? E-mail ingevuld + wachtwoord leeg
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_CorrectEmail_EmptyPassword()
        {
            Console.WriteLine("Test: CorrectEmail_EmptyPassword");

            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine("Loginpagina geladen.");

            loginPage.EnterEmail("gebruiker@example.com");
            Console.WriteLine("Correct e-mailadres ingevoerd.");

            loginPage.EnterPassword("");
            Console.WriteLine("Leeg wachtwoord ingevoerd.");

            loginPage.ClickLogin();
            Console.WriteLine("Loginpoging uitgevoerd.");

            var error = wait.Until(d => d.FindElement(By.Id("empty-input-error")));
            Console.WriteLine("Foutmelding gevonden: " + error.Text);

            Assert.AreEqual("Gegevens moeten ingevuld zijn", error.Text);
            Console.WriteLine(" Test geslaagd.\n");
        }

        // ------------------------
        // 6?? Beide leeg
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_BothFieldsEmpty()
        {
            Console.WriteLine("Test: BothFieldsEmpty");

            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine("Loginpagina geladen.");

            loginPage.EnterEmail("");
            Console.WriteLine("Leeg e-mailadres ingevoerd.");

            loginPage.EnterPassword("");
            Console.WriteLine("Leeg wachtwoord ingevoerd.");

            loginPage.ClickLogin();
            Console.WriteLine("Loginpoging uitgevoerd.");

            var error = wait.Until(d => d.FindElement(By.Id("empty-input-error")));
            Console.WriteLine("Foutmelding gevonden: " + error.Text);

            Assert.AreEqual("Gegevens moeten ingevuld zijn", error.Text);
            Console.WriteLine(" Test geslaagd.\n");
        }
    }
}
