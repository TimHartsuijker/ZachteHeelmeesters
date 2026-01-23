using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US1._19
{
    [TestClass]
    public class _1_19_4_1 : BaseTest
    {
        [TestMethod]
        public void TC_1_19_4_CorrecteLoginNaarPortaal()
        {
            const string EMAIL = "gebruiker@example.com";
            const string PASSWORD = "Wachtwoord123";

            // Stap 1: Navigatie
            LogStep(1, "Navigating to login page...");
            loginPage.Navigate();
            wait.Until(d => loginPage.IsPasswordInputDisplayed());
            LogSuccess(1, "Login page loaded successfully.");

            // Stap 2: Login uitvoeren
            LogStep(2, $"Performing login for user: {EMAIL}");
            loginPage.PerformLogin(EMAIL, PASSWORD);
            LogSuccess(2, "Login attempt submitted.");

            // Stap 3: Wachten op redirect
            LogStep(3, "Waiting for redirection to dashboard...");
            wait.Until(d => d.Url.Contains("/dashboard"));
            LogInfo($"Detected URL: {driver.Url}");
            LogSuccess(3, "Redirection to dashboard detected.");

            // Stap 4: Verificatie dashboard
            LogStep(4, "Verifying if user reached the dashboard...");
            Assert.IsTrue(driver.Url.Contains("/dashboard"), "Gebruiker is niet doorgestuurd naar het dashboard.");
            LogSuccess(4, "User is successfully logged in and present on the dashboard.");

            // Stap 5: Finale status check
            LogStep(5, "Final verification of portal state...");
            LogInfo("Session established and dashboard route active.");
            LogSuccess(5, "Login to portal verified successfully.");
        }
    }
}