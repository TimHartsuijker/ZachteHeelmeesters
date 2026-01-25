using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;
using System;



namespace SeleniumTests
{


    [TestClass]
    public class _3_13_2
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "http://localhost";
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
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void TC_3_13_2_AdminLoginSuccess()
        {
            Console.WriteLine("Test: Successful admin login");

           
            driver.Navigate().GoToUrl($"{baseUrl}/admin/login");
            Console.WriteLine("Admin login pagina geladen.");

           
            driver.FindElement(By.Id("admin-username"))
                  .SendKeys("admin@example.com");

            driver.FindElement(By.Id("admin-password"))
                  .SendKeys("Admin123");

            Console.WriteLine("Admin credentials ingevoerd.");

            
            driver.FindElement(By.Id("admin-login-btn")).Click();
            Console.WriteLine("Admin login uitgevoerd.");

           
            wait.Until(d => d.Url.Contains("/admin/dashboard"));

            Assert.IsTrue(
                driver.Url.Contains("/admin/dashboard"),
                "Admin is niet doorgestuurd naar het admin dashboard."
            );

            Console.WriteLine("? Admin succesvol ingelogd.\n");
        }
    }
}
