using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class TC_6_10_3_3
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
        public void TC_6_10_3_3_VerifyOptionalFieldsCanBeEmpty()
        {
            Console.WriteLine("Test started: TC_6_10_3_3_VerifyOptionalFieldsCanBeEmpty");

            driver.Navigate().GoToUrl($"{baseUrl}/create-user");

            IWebElement nameField = wait.Until(drv => drv.FindElement(By.Id("name")));
            IWebElement emailField = driver.FindElement(By.Id("email"));
            IWebElement roleDropdown = driver.FindElement(By.Id("role"));

            nameField.SendKeys("Jane Doe");
            emailField.SendKeys("jane.doe@test.com");
            roleDropdown.Click();

            IWebElement submitButton = driver.FindElement(By.XPath("//button[contains(text(),'Create Account')]"));
            submitButton.Click();

            wait.Until(drv => drv.Url.Contains("user-management"));

            Assert.IsTrue(driver.Url.Contains("user-management"),
                "Account was not created successfully when optional fields were empty.");

            Console.WriteLine("Account successfully created with only required fields.");
            Console.WriteLine("Test completed successfully.");
        }
    }
}
