using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US1._20
{
    [TestClass]
    public class _1_20 : BaseTest
    {
        private const string VALID_EMAIL = "gebruiker@example.com";
        private const string VALID_PASSWORD = "Wachtwoord123";

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            LogStep(0, "Initial setup: Navigating to login page...");
            loginPage.Navigate();
            wait.Until(d => loginPage.IsPasswordInputDisplayed());
            LogSuccess(0, "Setup complete: Login page ready.");
        }

        [TestMethod]
        public void TC_1_20_1_LogoutButtonIsAvailable()
        {
            LogStep(1, $"Logging in with user: {VALID_EMAIL}");
            loginPage.PerformLogin(VALID_EMAIL, VALID_PASSWORD);
            LogSuccess(1, "Login credentials submitted.");

            LogStep(2, "Waiting for dashboard to load...");
            wait.Until(d => dashboardPage.IsLogoutButtonDisplayed());
            LogSuccess(2, "Dashboard loaded.");

            LogStep(3, "Verifying logout button visibility...");
            Assert.IsTrue(dashboardPage.IsLogoutButtonDisplayed(), "ERROR: The logout button is not available on the Dashboardpage");
            LogSuccess(3, "Logout button is present and visible.");
        }

        [TestMethod]
        public void TC_1_20_2_NoAccessViaRouteAfterLoggingOut()
        {
            LogStep(1, "Logging in and waiting for dashboard...");
            loginPage.PerformLogin(VALID_EMAIL, VALID_PASSWORD);
            wait.Until(d => dashboardPage.IsLogoutButtonDisplayed());
            LogSuccess(1, "Authentication successful.");

            LogStep(2, "Clicking the logout button...");
            dashboardPage.ClickLogout();
            LogSuccess(2, "Logout command executed.");

            LogStep(3, "Attempting to navigate back to dashboard via URL routing...");
            dashboardPage.Navigate();
            LogInfo($"Current URL: {driver.Url}");

            LogStep(4, "Verifying access is denied...");
            Assert.IsFalse(dashboardPage.IsLogoutButtonDisplayed(), "ERROR: Dashboard was accessible via routing");
            LogSuccess(4, "Access denied. User is not on the dashboard.");
        }

        [TestMethod]
        public void TC_1_20_3_NoAccessNavigatingBackAfterLoggingOut()
        {
            LogStep(1, "Performing login...");
            loginPage.PerformLogin(VALID_EMAIL, VALID_PASSWORD);
            wait.Until(d => dashboardPage.IsLogoutButtonDisplayed());
            LogSuccess(1, "Session established.");

            LogStep(2, "Logging out of the application...");
            dashboardPage.ClickLogout();
            LogSuccess(2, "Session terminated.");

            LogStep(3, "Attempting to navigate back using browser history...");
            driver.Navigate().Back();
            LogInfo("Browser 'Back' command sent.");

            LogStep(4, "Verifying dashboard is not displayed...");
            Assert.IsFalse(dashboardPage.IsLogoutButtonDisplayed(), "ERROR: Dashboard was accessible via navigating back");
            LogSuccess(4, "Access blocked. Browser history navigation does not restore the session.");
        }

        [TestMethod]
        public void TC_1_20_4_UserRedirectedToLoginPageAfterLoggingOut()
        {
            LogStep(1, "Logging in to verify redirect behavior...");
            loginPage.PerformLogin(VALID_EMAIL, VALID_PASSWORD);
            wait.Until(d => dashboardPage.IsLogoutButtonDisplayed());
            LogSuccess(1, "Authenticated.");

            LogStep(2, "Clicking the logout button...");
            dashboardPage.ClickLogout();
            LogSuccess(2, "Logout clicked.");

            LogStep(3, "Waiting for automatic redirect to login page...");
            wait.Until(d => loginPage.IsPasswordInputDisplayed());
            LogInfo($"Redirected to: {driver.Url}");

            LogStep(4, "Verifying login page presence...");
            Assert.IsTrue(loginPage.IsPasswordInputDisplayed(), "ERROR: After logging out the user is not redirected to the Login page");
            LogSuccess(4, "User successfully redirected to the login page.");
        }
    }
}