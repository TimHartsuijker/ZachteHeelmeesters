using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _1_19_2
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
        public void TC_1_19_2_PasswordInputIsPresentAndAcceptsInput()
        {
            Console.WriteLine("Test gestart: TC_1_19_2_PasswordInputIsPresentAndAcceptsInput");

            Console.WriteLine("Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine("Loginpagina geladen.");

            
            Console.WriteLine("Controleren of wachtwoordveld zichtbaar is...");
            Assert.IsTrue(loginPage.IsPasswordFieldDisplayed(),
                "Het wachtwoordveld is niet zichtbaar.");
            Console.WriteLine("Wachtwoordveld is zichtbaar!");

            
            Console.WriteLine("Wachtwoordveld selecteren...");
            var passwordField = driver.FindElement(By.Id("password"));
            passwordField.Click();
            Console.WriteLine("Wachtwoordveld aangeklikt.");

            Assert.IsTrue(passwordField.Equals(driver.SwitchTo().ActiveElement()),
                "Het wachtwoordveld kan geen focus krijgen.");
            Console.WriteLine("Wachtwoordveld heeft focus.");

           
            string testPassword = "Test123!";
            Console.WriteLine($"Wachtwoord invoeren: {testPassword}");
            loginPage.EnterPassword(testPassword);

            
            Assert.AreEqual(testPassword, passwordField.GetAttribute("value"),
                "Het wachtwoordveld accepteert geen tekst.");
            Console.WriteLine("Wachtwoord correct ingevoerd!");

            Console.WriteLine("Test succesvol afgerond.");
        }

    }
}
