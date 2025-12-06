using OpenQA.Selenium;

namespace SeleniumTests.P_O_M
{
    public class LoginPage(IWebDriver driver)
    {
        private readonly IWebDriver driver = driver;

        // URL
        public static string Url => "http://localhost:5173/inloggen";

        // Locators
        private static By EmailInput => By.Id("email") ;
        private static By PasswordInput => By.Id("password");
        private static By LoginButton => By.Id("login-btn");
        private static By LoginHeader => By.XPath("//h1[contains(text(), 'Log in')]"); // Voor verificatie dat we op de pagina zijn

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
            Console.WriteLine($"Performing login with these credentials \nEmail: {email} \nPassword: {password}");
            EnterEmail(email);
            EnterPassword(password);
            ClickLogin();

            return new DashboardPage(driver);
        }

        // Verification
        public bool IsLoginPageDisplayed()
        {
            try
            {
                return driver.FindElement(LoginHeader).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        public bool IsEmailFieldDisplayed()
        {
            try
            {
                return driver.FindElement(EmailInput).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsPasswordFieldDisplayed()
        {
            try
            {
                return driver.FindElement(PasswordInput).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsLoginButtonDisplayed()
        {
            try
            {
                return driver.FindElement(LoginButton).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}