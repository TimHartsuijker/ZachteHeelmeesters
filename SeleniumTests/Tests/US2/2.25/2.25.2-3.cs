using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_25_2_3
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
        public void TC_2_25_2_3_OptionalFields_RemainEmpty_IfNotFilled()
        {
            Console.WriteLine("Test gestart: TC_2_25_2_3_OptionalFields_RemainEmpty_IfNotFilled");

            // Stap 1: Login
            driver.Navigate().GoToUrl($"{baseUrl}/login");
            loginPage.EnterEmail("patient_no_optional@example.com");
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();

            // Stap 2: Dashboard
            wait.Until(d => d.FindElement(By.Id("dashboard-container")));

            // Stap 3: Profielpagina openen
            wait.Until(d => d.FindElement(By.CssSelector("[data-test='nav-profile']"))).Click();

            // Stap 4: Controleren optionele velden
            string[] optionalFields =
            {
                "profile-emergency-contact",
            };

            foreach (var fieldId in optionalFields)
            {
                var element = wait.Until(d => d.FindElement(By.Id(fieldId)));
                string value = element.Text.Trim();

                Assert.IsTrue(string.IsNullOrWhiteSpace(value),
                    $"Optioneel veld '{fieldId}' zou leeg moeten zijn.");
            }

            Console.WriteLine("Optionele velden blijven correct leeg.");
        }
    }
}
