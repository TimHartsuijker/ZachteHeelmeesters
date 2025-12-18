using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _1_19_1
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "http://localhost:5173"; 

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

            Console.WriteLine("Test gestart: TC_1_19_1_EmailInputIsPresentAndAcceptsInput");

            Console.WriteLine("Navigeren naar loginpagina...");
            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine("Navigatie voltooid!");

            
            Console.WriteLine("Controleren of emailveld zichtbaar is...");
            wait.Until(drv => loginPage.IsEmailFieldDisplayed());
            Assert.IsTrue(loginPage.IsEmailFieldDisplayed(),
                "Het e-mailadres veld is niet zichtbaar.");
            Console.WriteLine("Emailveld is zichtbaar!");

          
            Console.WriteLine("Emailveld selecteren...");
            var emailField = driver.FindElement(By.Id("email"));
            emailField.Click();
            Console.WriteLine("Emailveld aangeklikt.");

            Assert.IsTrue(emailField.Equals(driver.SwitchTo().ActiveElement()),
                "Het e-mailadres veld kan geen focus krijgen.");
            Console.WriteLine("Emailveld heeft focus.");

            // Tekst invoeren
            string testEmail = "test@example.com";
            Console.WriteLine($"Email invoeren: {testEmail}");
            loginPage.EnterEmail(testEmail);

            Assert.AreEqual(testEmail, emailField.GetAttribute("value"),
                "Het e-mailadres veld accepteert geen tekst.");
            Console.WriteLine("Email correct ingevoerd!");

            Console.WriteLine("Test succesvol afgerond.");
        }
    }
}
