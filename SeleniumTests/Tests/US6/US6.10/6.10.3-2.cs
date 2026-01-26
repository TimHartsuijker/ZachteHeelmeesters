using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class TC_6_10_3_2
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "http://localhost:5173";

        [TestInitialize]
        public void Setup()
        {
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [TestCleanup]
        public void Cleanup()
        {
            driver.Quit();
        }

        [TestMethod]
        public void TC_6_10_3_2_VerifyRequiredFieldsValidation()
        {
            Console.WriteLine("Test started: TC_6_10_3_2_VerifyRequiredFieldsValidation");

            driver.Navigate().GoToUrl($"{baseUrl}/create-user");

            // Leave required fields empty and submit
            IWebElement submitButton = wait.Until(drv =>
                drv.FindElement(By.XPath("//button[contains(text(),'Create Account')]"))
            );

            submitButton.Click();
            Console.WriteLine("Create Account button clicked with empty required fields.");

            IWebElement errorMessage = wait.Until(drv =>
                drv.FindElement(By.ClassName("error-message"))
            );

            Assert.IsTrue(errorMessage.Displayed,
                "No error message displayed for empty required fields.");

            Console.WriteLine("Validation error message displayed correctly.");
            Console.WriteLine("Test completed successfully.");
        }
    }
}
