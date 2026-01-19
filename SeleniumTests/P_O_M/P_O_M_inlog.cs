using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests.P_O_M
{
    public class LoginPage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // URL van de inlogpagina
        public string Url => "http://localhost:5173/"; // Later vervangen

        private By EmailInput => By.Id("email");
        private By PasswordInput => By.Id("wachtwoord");
        private By LoginButton => By.Id("login-btn"); 


        public void Navigate()
        {
            driver.Navigate().GoToUrl(Url);
            
            // Give Vue time to mount and render
            System.Threading.Thread.Sleep(2000);
            
            // Wait for the email input to be present
            wait.Until(d => d.FindElement(EmailInput).Displayed);
        }

        public void EnterEmail(string email)
        {
            var input = wait.Until(d => d.FindElement(EmailInput));
            input.Clear();
            input.SendKeys(email);
        }

        public void EnterPassword(string password)
        {
            var input = wait.Until(d => d.FindElement(PasswordInput));
            input.Clear();
            input.SendKeys(password);
        }

        public void ClickLogin()
        {
            var button = wait.Until(d => d.FindElement(LoginButton));
            button.Click();
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
