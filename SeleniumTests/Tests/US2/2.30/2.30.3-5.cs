using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_30_3_5
    {
        private IWebDriver driver;
        private string baseUrl = "https://localhost:5173";
        private LoginPage loginPage;

        [TestInitialize]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            driver = new ChromeDriver(options);
            loginPage = new LoginPage(driver);
            Console.WriteLine("Setup voltooid.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void Dashboard_ShowsGracefulErrorOnDataFailure()
        {
            Console.WriteLine("Test gestart: Dashboard_ShowsGracefulErrorOnDataFailure");

            driver.Navigate().GoToUrl($"{baseUrl}/");

            // Simulatie: backend failure actief
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();

            var errorMessage = driver.FindElement(By.Id("data-error-message"));
            Assert.IsTrue(errorMessage.Text.Contains("Gegevens kunnen niet geladen worden"),
                "Gebruikersvriendelijke foutmelding ontbreekt.");

            Console.WriteLine("Dashboard toont foutmelding correct. Test geslaagd!");
        }
    }
}
