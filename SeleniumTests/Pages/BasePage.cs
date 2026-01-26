using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumTests.Pages
{
    public abstract class BasePage(IWebDriver driver)
    {
        protected readonly IWebDriver Driver = driver;
        protected readonly WebDriverWait Wait = new(driver, TimeSpan.FromSeconds(10));
        protected readonly string BaseUrl = "http://localhost";
        protected abstract string Path { get; }

        // === Gedeelde Acties ===
        public void NavigateTo(string url) => Driver.Navigate().GoToUrl(url);

        public bool IsElementDisplayed(By locator)
        {
            try
            {
                return Driver.FindElement(locator).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        protected void SendKeys(By locator, string text, bool clear = true)
        {
            var element = Driver.FindElement(locator);
            if (clear) element.Clear();
            element.SendKeys(text);
        }

        protected void SelectByValue(By locator, string value)
        {
            var selectElement = new SelectElement(Driver.FindElement(locator));
            selectElement.SelectByValue(value);
        }

        protected void Click(By locator) => Driver.FindElement(locator).Click();

        protected string GetText(By locator) => Driver.FindElement(locator).Text;

        // === Wacht Logica ===
        public void WaitForElement(By locator)
        {
            Wait.Until(d => d.FindElement(locator).Displayed);
        }

        // Wacht tot een element tekst bevat (handig voor error messages)
        protected bool WaitForElementToHaveText(By locator)
        {
            return Wait.Until(d => !string.IsNullOrEmpty(d.FindElement(locator).Text));
        }

        // Wacht tot een dropdown meer dan X opties heeft
        protected void WaitForDropdownToPopulate(By locator, int minCount = 1)
        {
            Wait.Until(d => new SelectElement(d.FindElement(locator)).Options.Count > minCount);
        }

        // Generieke JavaScript executor (voor die AddInvalidSpecialist methode)
        protected void ExecuteJs(string script, params object[] args)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript(script, args);
        }
    }
}