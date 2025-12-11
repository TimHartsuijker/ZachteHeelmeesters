using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_25_3_1
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
        public void TC_2_25_3_1_ProfileFields_AreReadOnly()
        {
            Console.WriteLine("Test gestart: TC_2_25_3_1_ProfileFields_AreReadOnly");

            // Stap 1: Login
            Console.WriteLine("Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");

            Console.WriteLine("Inloggegevens invullen...");
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();

            // Stap 2: Dashboard laden
            Console.WriteLine("Wachten op dashboard...");
            wait.Until(d => d.FindElement(By.Id("dashboard-container")));

            // Stap 3: Profielpagina openen
            Console.WriteLine("Navigeren naar profielpagina...");
            wait.Until(d => d.FindElement(By.CssSelector("[data-test='nav-profile']"))).Click();
            wait.Until(d => d.FindElement(By.Id("profile-page-container")));

            // Stap 4: Controleren of velden read-only zijn
            Console.WriteLine("Controleren of alle velden niet bewerkbaar zijn...");

            string[] profileFieldIds =
            {
                "profile-name",
                //"profile-address", (moet uitgebreider)
                "profile-birthdate",
                "profile-phone",
                "profile-email",
                "profile-emergency-contact",
            };

            foreach (var fieldId in profileFieldIds)
            {
                var field = driver.FindElement(By.Id(fieldId));

                try
                {
                    // Proberen te typen in het veld
                    field.SendKeys("TEST");
                    Assert.Fail($"Veld '{fieldId}' is bewerkbaar, maar zou read-only moeten zijn.");
                }
                catch (InvalidElementStateException)
                {
                    // Exception betekent veld is read-only → OK
                    Console.WriteLine($"✓ Veld '{fieldId}' is read-only.");
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Onverwachte fout bij veld '{fieldId}': {ex.Message}");
                }
            }

            Console.WriteLine("Alle profielvelden zijn correct read-only.");
        }
    }
}
