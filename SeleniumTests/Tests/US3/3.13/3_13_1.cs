using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US3._13
{
    [TestClass]
    public class _3_13_1 : BaseTest
    {
        [TestMethod]
        public void TC_3_13_1_AccessAdminLoginPage()
        {
            // Stap 1: Navigatie naar reguliere login
            LogStep(1, "Navigating to the standard user login page...");
            loginPage.Navigate();
            wait.Until(d => loginPage.IsPasswordInputDisplayed());
            LogSuccess(1, "Standard login page loaded successfully.");

            // Stap 2: Controleer aanwezigheid Admin link
            LogStep(2, "Verifying presence of the Admin Login link...");
            Assert.IsTrue(loginPage.IsAdminLoginLinkDisplayed(),
                "Admin login link is not visible on the user login page.");
            LogSuccess(2, "Admin login link is visible.");

            // Stap 3: Navigatie naar Admin login
            LogStep(3, "Clicking the Admin Login link and waiting for redirect...");
            loginPage.ClickAdminLogin();
            wait.Until(d => adminLoginPage.IsPasswordInputDisplayed());
            LogSuccess(3, "Admin login page reached.");

            // Stap 4: Verifieer URL scheiding
            LogStep(4, "Verifying that the admin login page is correctly separated by route...");

            // Controleer of de URL specifiek de admin route bevat
            Assert.IsTrue(driver.Url.Contains("/admin/login"),
                "User was not redirected to the correct admin login URL.");

            // Controleer of we niet meer op de reguliere login route zitten
            Assert.IsFalse(driver.Url.EndsWith("/login") && !driver.Url.Contains("/admin"),
                "Admin login page is not clearly separated from the user login page.");

            LogInfo($"Final Admin URL: {driver.Url}");
            LogSuccess(4, "Admin login page is distinct and successfully separated from the user portal.");
        }
    }
}