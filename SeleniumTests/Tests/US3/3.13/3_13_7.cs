using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _3_13_7
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
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }

       
        [TestMethod]
        public void TC_3_13_4_NonAdminCannotLoginAdminPage()
        {
            Console.WriteLine("Test: Successful admin login");

            // Step 1: Navigate to admin login page
            driver.Navigate().GoToUrl($"{baseUrl}/admin/login");
            Console.WriteLine("Admin login pagina geladen.");


            // Step 3: Click login
            driver.FindElement(By.Id("admin-login-btn")).Click();
            Console.WriteLine("Admin login uitgevoerd.");

            // Stap 3: Wacht op foutmelding
            var error = wait.Until(d => d.FindElement(By.Id("error-text")));
            Console.WriteLine("Foutmelding gevonden: " + error.Text);

            // Stap 4: Controleer dat login geweigerd is
            Assert.AreEqual("Gegevens moeten ingevuld zijn", error.Text);
            Console.WriteLine("Reguliere user is correct geweigerd bij admin login.");
        }
    }
}
