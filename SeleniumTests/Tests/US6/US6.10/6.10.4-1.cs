using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class TC_6_10_4_1
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
        public void TC_6_10_4_1_VerifyNewAccountIsSaved()
        {
            Console.WriteLine("Test started: TC_6_10_4_1_VerifyNewAccountIsSaved");

            // Step 1: Open create account page
            driver.Navigate().GoToUrl($"{baseUrl}/create-user");

            // Step 2: Fill in valid user data
            string testEmail = $"new.user{DateTime.Now.Ticks}@test.com";

            driver.FindElement(By.Id("name")).SendKeys("New User");
            driver.FindElement(By.Id("email")).SendKeys(testEmail);

            IWebElement roleDropdown = driver.FindElement(By.Id("role"));
            roleDropdown.Click();
            roleDropdown.SendKeys("Specialist");

            Console.WriteLine("Valid user data entered.");

            // Step 3: Save the account
            driver.FindElement(By.XPath("//button[contains(text(),'Create Account')]")).Click();
            Console.WriteLine("Create Account button clicked.");

            // Step 4: Navigate to user overview
            wait.Until(drv => drv.Url.Contains("user-management"));
            Console.WriteLine("User overview page opened.");

            // Step 5: Verify new account is visible
            IWebElement createdUser = wait.Until(drv =>
                drv.FindElement(By.XPath($"//*[contains(text(),'{testEmail}')]"))
            );

            Assert.IsTrue(createdUser.Displayed,
                "The newly created account is not visible in the user overview.");

            Console.WriteLine("New account successfully saved and visible.");
            Console.WriteLine("Test completed successfully.");
        }
    }
}
