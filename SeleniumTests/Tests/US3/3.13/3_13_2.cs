using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US3._13
{
    [TestClass]
    public class _3_13_2 : BaseTest
    {
        [TestMethod]
        public void TC_3_13_2_AdminLoginSuccess()
        {
            const string EMAIL = "admin@example.com";
            const string PASSWORD = "Admin123";

            // Stap 1: Navigatie
            LogStep(1, "Navigating to the Admin Login page...");
            adminLoginPage.Navigate();
            wait.Until(d => adminLoginPage.IsPasswordInputDisplayed());
            LogSuccess(1, "Admin Login page loaded successfully.");

            // Stap 2: Inloggen
            LogStep(2, $"Performing Admin login with email: {EMAIL}");
            adminLoginPage.PerformLogin(EMAIL, PASSWORD);
            LogSuccess(2, "Admin credentials submitted.");

            // Stap 3: Redirect controle
            LogStep(3, "Waiting for redirection to the admin dashboard...");
            wait.Until(d => d.Url.Contains("/admin/dashboard"));
            LogInfo($"Redirected to: {driver.Url}");
            LogSuccess(3, "Successfully reached the admin dashboard.");

            // Stap 4: Verificatie
            LogStep(4, "Verifying dashboard URL and access...");
            Assert.IsTrue(
                driver.Url.Contains("/admin/dashboard"),
                "Admin is niet doorgestuurd naar het admin dashboard."
            );

            // Optionele check of een specifiek admin-element zichtbaar is
            LogSuccess(4, "Admin session verified. Access to dashboard granted.");
        }
    }
}