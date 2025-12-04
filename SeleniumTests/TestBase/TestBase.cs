using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace SeleniumTests.TestBase
{
    public class TestBaseClass
    {
        protected IWebDriver driver;  // must be protected so subclasses can access it
        protected string BaseUrl = "http://localhost:5000";

        [TestInitialize]
        public void TestInitialize()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            try { driver.Quit(); } catch { }
        }
    }
}
