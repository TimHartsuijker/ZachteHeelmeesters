using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class TC_6_10_3_1
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
        public void TC_6_10_3_1_VerifyValidInputCanBeEntered()
        {
            Console.WriteLine("Test started: TC_6_10_3_1_VerifyValidInputCanBeEntered");

            driver.Navigate().GoToUrl($"{baseUrl}/create-user");

            IWebElement nameField = wait.Until(drv => drv.FindElement(By.Id("name")));
            IWebElement emailField = driver.FindElement(By.Id("email"));
            IWebElement roleDropdown = driver.FindElement(By.Id("role"));

            nameField.SendKeys("John Doe");
            emailField.SendKeys("john.doe@test.com");
            roleDropdown.Click();

            Console.WriteLine("All required fields filled with valid data.");

            Assert.AreEqual("John Doe", nameField.GetAttribute("value"));
            Assert.AreEqual("john.doe@test.com", emailField.GetAttribute("value"));

            Console.WriteLine("All data entered without errors.");
            Console.WriteLine("Test completed successfully.");
        }
    }
}
