using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using SeleniumTests.Base;
using SeleniumTests.Pages;

namespace US3._13
{
    [TestClass]
    public class _3_13_3 : BaseTest
    {
        [TestMethod]
        public void TC_3_13_3_AccessAdminFunctionalities()
        {
            const string ADMIN_EMAIL = "admin@example.com";
            const string ADMIN_PASSWORD = "Admin123";

            // Stap 1: Login
            LogStep(1, "Navigating to Admin Login and performing authentication...");
            adminLoginPage.Navigate();
            wait.Until(d => adminLoginPage.IsPasswordInputDisplayed());
            adminLoginPage.PerformLogin(ADMIN_EMAIL, ADMIN_PASSWORD);

            wait.Until(d => d.Url.Contains("/admin/dashboard"));
            LogSuccess(1, "Successfully logged in and reached Admin Dashboard.");

            // Stap 2: Interactie Gebruikersbeheer
            LogStep(2, "Testing 'Gebruikersbeheer' interaction...");
            wait.Until(d => dashboardPage.IsUserManagementLinkDisplayed());
            Assert.IsTrue(dashboardPage.IsUserManagementLinkDisplayed(), "Gebruikersbeheer knop niet zichtbaar.");
            dashboardPage.ClickUserManagementLink();
            LogSuccess(2, "User Management section is accessible.");

            // Stap 3: Interactie Instellingen
            LogStep(3, "Testing 'Instellingen' interaction...");
            wait.Until(d => dashboardPage.IsSettingsLinkDisplayed());
            Assert.IsTrue(dashboardPage.IsSettingsLinkDisplayed(), "Instellingen knop niet zichtbaar.");
            dashboardPage.ClickSettingsLink();
            LogSuccess(3, "System Settings section is accessible.");

            // Stap 4: Interactie Audit Logs
            LogStep(4, "Testing 'Audit logs' interaction...");
            wait.Until(d => dashboardPage.IsAuditLinkDisplayed());
            Assert.IsTrue(dashboardPage.IsAuditLinkDisplayed(), "Audit logs knop niet zichtbaar.");
            dashboardPage.ClickAuditLink();
            LogSuccess(4, "Audit Logs section is accessible.");

            // Finale verificatie
            LogStep(5, "Final verification of admin portal state...");
            LogInfo("✓ Administrative privileges confirmed via UI interaction.");
            LogSuccess(5, "Test TC_3_13_3 completed successfully.");
        }
    }
}