using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace SeleniumTests
{
    [TestClass]
    public class TC_6_10_5_1
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
        public void TC_6_10_5_1_VerifyUserTypeSelectionIsAvailable()
        {
            Console.WriteLine("Test started: TC_6_10_5_1_VerifyUserTypeSelectionIsAvailable");

            // Step 1: Open create account page
            driver.Navigate().GoToUrl($"{baseUrl}/create-user");

            // Step 2: Locate user type field
            IWebElement roleDropdownElement = wait.Until(drv =>
                drv.FindElement(By.Id("role"))
            );

            SelectElement roleDropdown = new SelectElement(roleDropdownElement);

            // Expected user types
            string[] expectedRoles = { "Specialist", "General Practitioner", "Administrator" };

            foreach (string role in expectedRoles)
            {
                bool roleExists = roleDropdown.Options.Any(option =>
                    option.Text.Equals(role, StringComparison.OrdinalIgnoreCase));

                Assert.IsTrue(roleExists,
                    $"User type '{role}' is not available in the user type selection.");
            }

            Console.WriteLine("All required user types are selectable.");
            Console.WriteLine("Test completed successfully.");
        }
    }
}
