using OpenQA.Selenium;

namespace SeleniumTests.P_O_M
{
    public class LoginPage
    {
        private readonly IWebDriver driver;

        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // URL van de inlogpagina
        public string Url => "http://localhost:5173/"; // Later vervangen

        private By EmailInput => By.Id("email");
        private By PasswordInput => By.Id("wachtwoord");
        private By LoginButton => By.Id("login-btn"); 

     
        public void Navigate()
        {
            driver.Navigate().GoToUrl(Url);
        }

        public void EnterEmail(string email)
        {
            var input = driver.FindElement(EmailInput);
            input.Clear();
            input.SendKeys(email);
        }

        public void EnterPassword(string password)
        {
            var input = driver.FindElement(PasswordInput);
            input.Clear();
            input.SendKeys(password);
        }

        public void ClickLogin()
        {
            driver.FindElement(LoginButton).Click();
        }

        public void LoginAsPatient(string email, string password)
        {
            Navigate();
            EnterEmail(email);
            EnterPassword(password);
            ClickLogin();
        }

        public bool IsEmailFieldDisplayed()
        {
            return driver.FindElement(EmailInput).Displayed;
        }

        public bool IsPasswordFieldDisplayed()
        {
            return driver.FindElement(PasswordInput).Displayed;
        }

        public bool IsLoginButtonDisplayed()
        {
            return driver.FindElement(LoginButton).Displayed;
        }
    }
}
