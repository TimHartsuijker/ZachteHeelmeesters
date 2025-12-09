using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_1_4
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
        public void TC_2_30_1_4_WelcomeMessageWithoutName()
        {
            Console.WriteLine("Test gestart: TC_2_30_1_4_WelcomeMessageWithoutName");

            // Stap 1: Navigeren naar loginpagina
            Console.WriteLine("Stap 1: Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");
            Console.WriteLine("Navigatie voltooid!");

            // Stap 2: Inloggen met account zonder naam
            Console.WriteLine("Stap 2: Inloggen met account zonder naam...");
            loginPage.EnterEmail("noname@example.com");
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();
            Console.WriteLine("Login verstuurd!");

            // Stap 3: Wachten tot dashboard geladen is
            Console.WriteLine("Stap 3: Dashboard laden...");
            wait.Until(d => d.FindElement(By.Id("dashboard-container")));
            Console.WriteLine("Dashboard succesvol geladen!");

            // Stap 4: Controleren welkomstboodschap
            Console.WriteLine("Stap 4: Controleren welkomstboodschap...");
            var welcome = driver.FindElement(By.XPath("//*[contains(text(),'Welkom')]"));
            string text = welcome.Text;

            Assert.IsFalse(text.Contains("@") || text.Contains("null"),
                "Fallback welkomsttekst bevat ongeldige placeholders.");
            Assert.IsTrue(text.Trim().Equals("Welkom!") || text.Trim() == "Welkom",
                $"Onverwachte tekst: {text}");

            Console.WriteLine("PASS: Welkomstboodschap zonder naam correct weergegeven.");
            Console.WriteLine("Test succesvol afgerond.");
        }
    }
}
