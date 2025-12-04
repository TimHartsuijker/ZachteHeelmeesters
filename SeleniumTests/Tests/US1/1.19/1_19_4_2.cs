using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _1_19_4_NoLogin
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "https://localhost:7058";

        [TestInitialize]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [TestCleanup]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void TC_1_19_4_NoLogin_AccessDashboard()
        {
            // Probeer direct dashboard te openen
            driver.Navigate().GoToUrl($"{baseUrl}/dashboard");

            // Wacht tot loginpagina zichtbaar is
            var loginField = wait.Until(d => d.FindElement(By.Id("email")));

            // Controleer dat gebruiker terug naar loginpagina wordt gestuurd
            Assert.IsTrue(loginField.Displayed, "Gebruiker kan dashboard openen zonder inloggen.");
        }
    }
}
