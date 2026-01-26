using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Drawing;

namespace US2._30
{
    [TestClass]
    public class _2_30_3_3
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "http://localhost";

        [TestInitialize]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            Console.WriteLine("Setup completed.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void Dashboard_OverviewReadable()
        {
            Console.WriteLine("Test started: Dashboard_OverviewReadable");
            Console.WriteLine("Test case: TC2.30.3-3 - Overview is clear and quickly readable");

            // Step 1: Navigate to login page and log in
            Console.WriteLine("Step 1: Navigating to login page and logging in...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");

            driver.FindElement(By.Id("email")).SendKeys("gebruiker@example.com");
            driver.FindElement(By.Id("wachtwoord")).SendKeys("Wachtwoord123");

            Console.WriteLine("Step 1b: Logging in...");
            driver.FindElement(By.Id("login-btn")).Click();

            // Step 2: Wait for dashboard to load
            Console.WriteLine("Step 2: Waiting for dashboard...");
            wait.Until(d => d.FindElement(By.CssSelector("[data-test='welcome-message']")));
            Console.WriteLine("Dashboard loaded!");

            // Step 3: Evaluate layout, grouping, and readability
            Console.WriteLine("Step 3: Evaluating layout, grouping, and readability...");

            Console.WriteLine("3.1: Checking grouping...");
            CheckGrouping();

            Console.WriteLine("3.2: Checking scannability...");
            CheckScannability();

            Console.WriteLine("3.3: Checking simplicity...");
            CheckSimplicity();

            Console.WriteLine("Dashboard overview is clear and quickly readable. Test passed!");
        }

        private void CheckGrouping()
        {
            var welcomeBox = driver.FindElement(By.ClassName("welcome-box"));
            var dashboardOverview = driver.FindElement(By.ClassName("dashboard-overzicht"));
            var dashboardGrid = driver.FindElement(By.ClassName("dashboard-grid"));

            Assert.IsTrue(welcomeBox.Displayed, "Welcome box is not visible.");
            Assert.IsTrue(dashboardOverview.Displayed, "Dashboard overview section is not visible.");
            Assert.IsTrue(dashboardGrid.Displayed, "Dashboard grid is not visible.");

            var panels = driver.FindElements(By.CssSelector(".dashboard-grid > div"));
            Assert.IsTrue(panels.Count >= 2, "Dashboard has fewer than 2 panels (insufficient grouping).");

            foreach (var panel in panels)
            {
                var margin = panel.GetCssValue("margin");
                var padding = panel.GetCssValue("padding");
                Console.WriteLine($"Panel margin: {margin}, padding: {padding}");

                Assert.IsFalse(string.IsNullOrWhiteSpace(margin) && string.IsNullOrWhiteSpace(padding),
                    "Panel has no visual separation (no margin/padding).");
            }

            Console.WriteLine("✓ Overview is clearly grouped.");
        }

        private void CheckScannability()
        {
            var welcomeText = driver.FindElement(By.CssSelector("[data-test='welcome-message']"));
            var welcomeSize = welcomeText.GetCssValue("font-size");
            Console.WriteLine($"Welcome text font-size: {welcomeSize}");

            if (welcomeSize.EndsWith("px"))
            {
                var fontSize = int.Parse(welcomeSize.Replace("px", ""));
                Assert.IsTrue(fontSize >= 20,
                    $"Welcome text is too small ({fontSize}px) for quick recognition.");
            }

            var panels = driver.FindElements(By.CssSelector(".dashboard-grid > div"));
            foreach (var panel in panels)
            {
                var panelTitle = panel.FindElement(By.TagName("h2"));
                var titleSize = panelTitle.GetCssValue("font-size");

                Console.WriteLine($"Panel title: '{panelTitle.Text}', font-size: {titleSize}");

                if (titleSize.EndsWith("px"))
                {
                    var titleFontSize = int.Parse(titleSize.Replace("px", ""));
                    Assert.IsTrue(titleFontSize >= 16,
                        $"Panel title '{panelTitle.Text}' is too small ({titleFontSize}px).");
                }
            }

            var panelContents = driver.FindElements(By.CssSelector(".dashboard-grid > div > p"));
            foreach (var content in panelContents)
            {
                Assert.IsTrue(content.Text.Length < 200,
                    $"Panel content is too long ({content.Text.Length} characters).");
            }

            Console.WriteLine("✓ Overview is directly scannable.");
        }

        private void CheckSimplicity()
        {
            var panels = driver.FindElements(By.CssSelector(".dashboard-grid > div"));
            Assert.IsTrue(panels.Count <= 4,
                $"Too many panels ({panels.Count}), overview is not simple enough.");

            Console.WriteLine($"Number of panels: {panels.Count} (acceptable)");

            foreach (var panel in panels)
            {
                var allText = panel.Text;
                var lines = allText.Split('\n').Length;

                Assert.IsTrue(lines <= 10,
                    $"Panel has too much text ({lines} lines).");
                Assert.IsTrue(allText.Length < 500,
                    $"Panel has too much text ({allText.Length} characters).");
            }

            var elements = driver.FindElements(By.CssSelector(".dashboard-grid *"));
            Assert.IsTrue(elements.Count < 50,
                $"Dashboard grid has too many elements ({elements.Count}), too complex.");

            Console.WriteLine("✓ Overview is simple and limited.");
        }
    }
}
