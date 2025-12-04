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
            
            driver.Navigate().GoToUrl($"{baseUrl}/");

            
            Assert.IsTrue(loginPage.IsPasswordFieldDisplayed(),
                "Het wachtwoordveld is niet zichtbaar.");

            
            var passwordField = driver.FindElement(By.Id("password"));
            passwordField.Click();
            Assert.IsTrue(passwordField.Equals(driver.SwitchTo().ActiveElement()),
                "Het wachtwoordveld kan geen focus krijgen.");

           
            string testPassword = "Test123!";
            loginPage.EnterPassword(testPassword);

            Assert.AreEqual(testPassword, passwordField.GetAttribute("value"),
                "Het wachtwoordveld accepteert geen tekst.");
        }
    }
}
