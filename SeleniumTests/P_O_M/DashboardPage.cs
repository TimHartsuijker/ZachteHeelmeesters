using OpenQA.Selenium;

namespace SeleniumTests.Pages
{
    public class DashboardPage (IWebDriver driver)
    {
        private readonly IWebDriver driver = driver;

        // URL
        public string Url => "http://localhost:5000/dashboard";

        // Locators
        private By DashboardHeader => By.Id("header");
        private By LogoutButton => By.Id("logout-btn");

        // Actions
        public void ClickLogout()
        {
            driver.FindElement(LogoutButton).Click();
        }

        public void NavigateToDashboard()
        {
            driver.Navigate().GoToUrl(Url);
        }

        // Verifications

        public bool IsLogoutButtonDisplayed()
        {
            try
            {
                return driver.FindElement(LogoutButton).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsDashboardDisplayed()
        {
            try
            {
                return driver.FindElement(DashboardHeader).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}