using OpenQA.Selenium;
using System.Security.Cryptography.X509Certificates;

namespace SeleniumTests.Pages
{
    public class DashboardPage(IWebDriver driver) : BasePage(driver)
    {
        protected override string Path => "/dashboard";

        #region Locators
        private static By LogoutButton => By.Id("logout-btn");

        // Admin
        private static By UserManagementLink => By.Id("user-management-link");
        private static By SettingsLink => By.Id("settings-link");
        private static By AuditLink => By.Id("audit-link");

        // Patient
        private static By WelcomeMessage => By.CssSelector("[data-test='welcome-message']");
        private static By DashboardGrid => By.ClassName("dashboard-grid");
        private static By LeftPanel => By.ClassName("panel-left");
        private static By RightPanel => By.ClassName("panel-right");
        private static By GenericPanels => By.CssSelector(".dashboard-grid > div");
        private static By DashboardLink => By.XPath("//a[contains(text(),'Dashboard')]");
        private static By AppointmentsLink => By.XPath("//a[contains(text(),'Mijn afspraken')]");
        private static By MedicalDossierLink => By.XPath("//a[contains(text(),'Mijn medisch dossier')]");
        private static By ProfileLink => By.XPath("//a[contains(text(),'Mijn profiel')]");
        private static By Navbar => By.XPath("//nav[contains(@class, 'navbar')]");

        #endregion

        #region Actions
        
        public void Navigate() => NavigateTo(BaseUrl + Path);

        // Admin
        public void ClickLogout() => Click(LogoutButton);
        public void ClickUserManagementLink() => Click(UserManagementLink);
        public void ClickSettingsLink() => Click(SettingsLink);
        public void ClickAuditLink() => Click(AuditLink);

        // Patient 
        public void ClickAppointments() => Click(AppointmentsLink);
        public void ClickMedicalDossier() => Click(MedicalDossierLink);
        public void ClickProfile() => Click(ProfileLink);
        public void ClickDashboardLink() => Click(DashboardLink);

        #endregion

        // Verifications
        public bool IsLogoutButtonDisplayed() => IsElementDisplayed(LogoutButton);
        public bool IsUserManagementLinkDisplayed() => IsElementDisplayed(UserManagementLink);
        public bool IsSettingsLinkDisplayed() => IsElementDisplayed(SettingsLink);
        public bool IsAuditLinkDisplayed() => IsElementDisplayed(AuditLink);

        public string GetWelcomeMessageText() => driver.FindElement(WelcomeMessage).Text;
        public bool IsWelcomeMessageDisplayed() => IsElementDisplayed(WelcomeMessage);
        public bool IsDashboardGridDisplayed() => IsElementDisplayed(DashboardGrid);
        public bool IsNavbarVisible() => IsElementDisplayed(Navbar);

        public string GetLeftPanelTitle() => driver.FindElement(LeftPanel).FindElement(By.TagName("h2")).Text;
        public string GetRightPanelTitle() => driver.FindElement(RightPanel).FindElement(By.TagName("h2")).Text;

        public int GetPanelCount() => driver.FindElements(GenericPanels).Count;

        public IWebElement GetWelcomeMessageElement() => driver.FindElement(WelcomeMessage);

        // Helper voor de link-verificatie test
        public bool IsUrlCorrect(string expectedPart) => driver.Url.Contains(expectedPart);
    }
}