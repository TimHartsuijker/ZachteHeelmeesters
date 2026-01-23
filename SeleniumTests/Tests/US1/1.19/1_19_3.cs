using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US1._19
{
    [TestClass]
    public class _1_19_3 : BaseTest
    {
        [TestMethod]
        public void TC_1_19_3_ValidatieCorrecteInlog()
        {
            const string EMAIL = "gebruiker@example.com";
            const string PASSWORD = "Wachtwoord123";

            // Stap 1: Navigatie
            LogStep(1, "Navigating to login page...");
            loginPage.Navigate();
            wait.Until(d => loginPage.IsPasswordInputDisplayed());
            LogSuccess(1, "Login page loaded successfully.");

            // Stap 2: Inloggen
            LogStep(2, $"Attempting login with email: {EMAIL}");
            loginPage.PerformLogin(EMAIL, PASSWORD);
            LogSuccess(2, "Login credentials submitted.");

            // Stap 3: Redirect controle
            LogStep(3, "Waiting for dashboard redirect...");
            wait.Until(d => dashboardPage.IsLogoutButtonDisplayed() || !d.Url.EndsWith("/login"));
            LogInfo($"Current URL after redirect: {driver.Url}");
            LogSuccess(3, "Redirect detected successfully.");

            // Stap 4: Verificatie
            LogStep(4, "Verifying user is no longer on the login page...");
            Assert.IsFalse(driver.Url.Contains("login"), "Gebruiker bleef op loginpagina na correcte inlog.");
            LogSuccess(4, "User successfully redirected. Login validation passed.");

            // Stap 5: Finale controle dashboard
            LogStep(5, "Verifying dashboard elements...");
            Assert.IsTrue(dashboardPage.IsLogoutButtonDisplayed(), "Logout knop niet gevonden op dashboard.");
            LogSuccess(5, "Dashboard is fully loaded and functional.");
        }
    }
}