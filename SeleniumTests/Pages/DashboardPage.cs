using OpenQA.Selenium;
using System.Security.Cryptography.X509Certificates;

namespace SeleniumTests.Pages
{
    public class DashboardPage(IWebDriver driver) : BasePage(driver)
    {
        public static string Path => "/dashboard";

        // Locators
        private static By LogoutButton => By.Id("logout-btn");
        private static By UserManagementLink => By.Id("user-management-link");
        private static By SettingsLink => By.Id("settings-link");
        private static By AuditLink => By.Id("audit-link");

        // Actions
        public void Navigate() => NavigateTo(BaseUrl + Path);
        public void ClickLogout() => Click(LogoutButton);
        public void ClickUserManagementLink() => Click(UserManagementLink);
        public void ClickSettingsLink() => Click(SettingsLink);
        public void ClickAuditLink() => Click(AuditLink);

        // Verifications
        public bool IsLogoutButtonDisplayed() => IsElementDisplayed(LogoutButton);
        public bool IsUserManagementLinkDisplayed() => IsElementDisplayed(UserManagementLink);
        public bool IsSettingsLinkDisplayed() => IsElementDisplayed(SettingsLink);
        public bool IsAuditLinkDisplayed() => IsElementDisplayed(AuditLink);
    }
}