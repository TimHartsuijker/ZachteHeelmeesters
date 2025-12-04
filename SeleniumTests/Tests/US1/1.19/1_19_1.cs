using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _1_19_1
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
        public void TC_1_19_1_EmailInputIsPresentAndAcceptsInput()
        {
            
            driver.Navigate().GoToUrl($"{baseUrl}/");

            
            Assert.IsTrue(loginPage.IsEmailFieldDisplayed(),
                "Het e-mailadres veld is niet zichtbaar.");

           
            var emailField = driver.FindElement(By.Id("email"));
            emailField.Click();
            Assert.IsTrue(emailField.Equals(driver.SwitchTo().ActiveElement()),
                "Het e-mailadres veld kan geen focus krijgen.");

            
            string testEmail = "test@example.com";
            loginPage.EnterEmail(testEmail);

            Assert.AreEqual(testEmail, emailField.GetAttribute("value"),
                "Het e-mailadres veld accepteert geen tekst.");
        }
    }
}
