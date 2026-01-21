using OpenQA.Selenium;

namespace SeleniumTests.P_O_M
{
    public class LoginPage(IWebDriver driver)
    {
        private readonly IWebDriver driver = driver;

        // URL
        public static string Url => "http://localhost/login";

        // Locators
        private static By EmailInput => By.Id("email") ;
        private static By PasswordInput => By.Id("password");
        private static By LoginButton => By.Id("login-btn");
        private static By LoginHeader => By.XPath("//h1[contains(text(), 'Log in')]"); // Voor verificatie dat we op de pagina zijn
        private static By AdminLoginLink => By.Id("admin-login-link");
        // Actions

        public void Navigate() => driver.Navigate().GoToUrl(Url);

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

        public void ClickLogin() => driver.FindElement(LoginButton).Click();
        public void ClickAdminLogin() => driver.FindElement(AdminLoginLink).Click();
        public DashboardPage PerformLogin(string email, string password)
        {
            Console.WriteLine($"Performing login with these credentials \nEmail: {email} \nPassword: {password}");
            EnterEmail(email);
            EnterPassword(password);
            ClickLogin();

            return new DashboardPage(driver);
        }

        // Verification
        public bool IsAdminLoginLinkDisplayed()
        {
            return driver.FindElement(AdminLoginLink).Displayed;
        }

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