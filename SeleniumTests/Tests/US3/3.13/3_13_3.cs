using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M; // Als je een Page Object Model hebt
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _3_13_3
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "http://localhost:5173";
        private LoginPage loginPage; // POM voor login

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
        public void TC_3_13_3_AccessAdminFunctionalities()
        {
            Console.WriteLine("Test gestart: TC-3.13.3 Access admin functionalities");

           
            driver.Navigate().GoToUrl($"{baseUrl}/admin/login");
            Console.WriteLine("Admin login pagina geladen.");

            
            driver.FindElement(By.Id("admin-username")).SendKeys("admin@example.com");
            driver.FindElement(By.Id("admin-password")).SendKeys("Admin123");
            driver.FindElement(By.Id("admin-login-btn")).Click();
            Console.WriteLine("Admin login uitgevoerd.");

           
            wait.Until(d => d.Url.Contains("/admin/dashboard"));
            Console.WriteLine("Admin dashboard geladen.");

            
            var gebruikersBtn = wait.Until(d => d.FindElement(By.XPath("//button[text()='Gebruikersbeheer']")));
            var instellingenBtn = wait.Until(d => d.FindElement(By.XPath("//button[text()='Instellingen']")));
            var auditBtn = wait.Until(d => d.FindElement(By.XPath("//button[text()='Audit logs']")));

            Assert.IsTrue(gebruikersBtn.Displayed, "Gebruikersbeheer knop niet zichtbaar.");
            Assert.IsTrue(instellingenBtn.Displayed, "Instellingen knop niet zichtbaar.");
            Assert.IsTrue(auditBtn.Displayed, "Audit logs knop niet zichtbaar.");

            Console.WriteLine("Alle functionaliteiten zijn zichtbaar.");

          
            gebruikersBtn.Click();
            Console.WriteLine("Gebruikersbeheer geopend.");

            instellingenBtn.Click();
            Console.WriteLine("Instellingen geopend.");

            auditBtn.Click();
            Console.WriteLine("Audit logs geopend.");

            Console.WriteLine("? TC-3.13.3 succesvol uitgevoerd.");
        }
    }
}
