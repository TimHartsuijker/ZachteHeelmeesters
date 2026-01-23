using OpenQA.Selenium;

namespace SeleniumTests.Pages
{
    public class AdminLoginPage(IWebDriver driver) : BasePage(driver)
    {
        public static string Path => "/admin/login";

        // Locators
        private static By EmailInput => By.Id("admin-username");
        private static By PasswordInput => By.Id("admin-password");
        private static By LoginButton => By.Id("admin-login-btn");
        private static By ErrorMessage => By.Id("error-text");

        public void Navigate() => NavigateTo(BaseUrl + Path);

        // Display Checks
        public bool IsEmailInputDisplayed() => IsElementDisplayed(EmailInput);
        public bool IsPasswordInputDisplayed() => IsElementDisplayed(PasswordInput);
        public bool IsLoginButtonDisplayed() => IsElementDisplayed(LoginButton);
        public bool IsErrorDisplayed() => IsElementDisplayed(ErrorMessage);

        public bool IsEmailInputFocused() => Driver.SwitchTo().ActiveElement().GetAttribute("id") == "admin-username";
        public bool IsPasswordInputFocused() => Driver.SwitchTo().ActiveElement().GetAttribute("id") == "admin-password";

        public void EnterEmail(string email) => SendKeys(EmailInput, email);
        public void EnterPassword(string password) => SendKeys(PasswordInput, password);

        public void ClickEmailInput() => Click(EmailInput);
        public void ClickPasswordInput() => Click(PasswordInput);

        public string GetEmailInputValue() => Driver.FindElement(EmailInput).GetAttribute("value") ?? string.Empty;
        public string GetPasswordInputValue() => Driver.FindElement(PasswordInput).GetAttribute("value") ?? string.Empty;
        public string GetErrorMessageText() => GetText(ErrorMessage);

        public void PerformLogin(string email, string password)
        {
            SendKeys(EmailInput, email);
            SendKeys(PasswordInput, password);
            Click(LoginButton);
        }

        public string WaitForError() => WaitForElementToHaveText(ErrorMessage) ? GetErrorMessageText() : string.Empty;
    }
}