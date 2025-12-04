using OpenQA.Selenium;
using SeleniumTests.Pages; // Veronderstel dat de volgende pagina in Pages staat

namespace SeleniumTests.Pages
{
    public class LoginPage(IWebDriver driver)
    {
        private readonly IWebDriver driver = driver;

        // URL
        public string Url => "http://localhost:5000/inloggen";

        // Locators
        private By EmailInput => By.Id("email") ;
        private By PasswordInput => By.Id("password");
        private By LoginButton => By.Id("login-btn");
        private By LoginHeader => By.XPath("//h1[contains(text(), 'Log in')]"); // Voor verificatie dat we op de pagina zijn

        // Actions

        public void Navigate()
        {
            driver.Navigate().GoToUrl(Url);
        }

        public void EnterEmail(string email)
        {
            driver.FindElement(EmailInput).SendKeys(email);
        }

        public void EnterPassword(string password)
        {
            driver.FindElement(PasswordInput).SendKeys(password);
        }

        public void ClickLogin()
        {
            driver.FindElement(LoginButton).Click();
        }

        public DashboardPage PerformLogin(string email, string password)
        {
            EnterEmail(email);
            EnterPassword(password);
            ClickLogin();

            return new DashboardPage(driver);
        }

        // Verification
        public bool IsLoginPageDisplayed()
        {
            // Controleer op een uniek element op de pagina (bijv. de header)
            return driver.FindElement(LoginHeader).Displayed;
        }

        public bool IsEmailFieldDisplayed() => driver.FindElement(EmailInput).Displayed;
        public bool IsPasswordFieldDisplayed() => driver.FindElement(PasswordInput).Displayed;
        public bool IsLoginButtonDisplayed() => driver.FindElement(LoginButton).Displayed;
    }
}