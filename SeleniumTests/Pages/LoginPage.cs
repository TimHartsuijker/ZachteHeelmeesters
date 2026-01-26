using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests.Pages
{
    public class LoginPage(IWebDriver driver) : BasePage(driver)
    {
        protected override string Path => "/login";

        // Locators
        private static By EmailInput => By.Id("email");
        private static By PasswordInput => By.Id("wachtwoord");
        private static By AdminLoginLink => By.Id("admin-login-link");
        private static By RegisterLink => By.Id("go-to-register");
        private static By LoginButton => By.Id("login-btn");
        private static By ErrorMessage => By.Id("login-error");
        private static By EmptyMessage => By.Id("empty-input-error");

        public void Navigate() => NavigateTo(BaseUrl + Path);

        // Display Checks
        public bool IsEmailInputDisplayed() => IsElementDisplayed(EmailInput);
        public bool IsPasswordInputDisplayed() => IsElementDisplayed(PasswordInput);
        public bool IsAdminLoginLinkDisplayed() => IsElementDisplayed(AdminLoginLink);
        public bool IsRegisterLinkDisplayed() => IsElementDisplayed(RegisterLink);
        public bool IsLoginButtonDisplayed() => IsElementDisplayed(LoginButton);
        public bool IsErrorDisplayed() => IsElementDisplayed(ErrorMessage);
        public bool IsEmptyDisplayed() => IsElementDisplayed(EmptyMessage);

        public bool IsEmailInputFocused() => Driver.SwitchTo().ActiveElement().GetAttribute("id") == "email";
        public bool IsPasswordInputFocused() => Driver.SwitchTo().ActiveElement().GetAttribute("id") == "wachtwoord";

        public void EnterEmail(string email) => SendKeys(EmailInput, email);
        public void EnterPassword(string password) => SendKeys(PasswordInput, password);

        public void ClickEmailInput() => Click(EmailInput);
        public void ClickPasswordInput() => Click(PasswordInput);
        public void ClickAdminLogin() => Click(AdminLoginLink);
        public void ClickRegisterLink() => Click(RegisterLink);

        public string GetEmailInputValue() => Driver.FindElement(EmailInput).GetAttribute("value") ?? string.Empty;
        public string GetPasswordInputValue() => Driver.FindElement(PasswordInput).GetAttribute("value") ?? string.Empty;
        public string GetErrorMessageText() => GetText(ErrorMessage);
        public string GetEmptyMessageText() => GetText(EmptyMessage);

        public void PerformLogin(string email, string password)
        {
            Wait.Until(d => IsPasswordInputDisplayed());
            SendKeys(EmailInput, email);
            SendKeys(PasswordInput, password);
            Click(LoginButton);
            HandlePasswordAlert();
        }

        public void HandlePasswordAlert()
        {
            try
            {
                var shortWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(3));
                IAlert alert = shortWait.Until(d => d.SwitchTo().Alert());

                alert.Accept();
            }
            catch (WebDriverTimeoutException)
            {
            }
            catch (NoAlertPresentException)
            {
            }
        }
    }
}