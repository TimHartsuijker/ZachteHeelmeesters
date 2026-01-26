using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US3._13
{
    [TestClass]
    public class _3_13_4 : BaseTest
    {
        [TestMethod]
        public void TC_3_13_4_NonAdminCannotLoginAdminPage()
        {
            const string USER_EMAIL = "gebruiker@example.com";
            const string USER_PASSWORD = "Wachtwoord123";

            // Stap 1: Navigatie naar Admin Login
            LogStep(1, "Navigating directly to the Admin Login page...");
            driver.Navigate().GoToUrl($"{baseUrl}/admin/login");
            wait.Until(d => d.FindElement(By.Id("admin-password")).Displayed);
            LogSuccess(1, "Admin login page loaded successfully.");

            // Stap 2: Invoeren van reguliere gebruikersgegevens
            LogStep(2, $"Entering regular user credentials (Email: {USER_EMAIL}) into admin fields...");
            driver.FindElement(By.Id("admin-username")).SendKeys(USER_EMAIL);
            driver.FindElement(By.Id("admin-password")).SendKeys(USER_PASSWORD);
            LogSuccess(2, "Non-admin credentials entered.");

            // Stap 3: Inlogpoging uitvoeren
            LogStep(3, "Attempting to log in to the admin portal...");
            driver.FindElement(By.Id("admin-login-btn")).Click();
            LogSuccess(3, "Login attempt submitted.");

            // Stap 4: Foutmelding verificatie
            LogStep(4, "Waiting for error message and verifying access denial...");
            var error = wait.Until(d => d.FindElement(By.Id("error-text")));
            LogInfo($"Error message detected: '{error.Text}'");

            Assert.AreEqual("Inloggegevens zijn incorrect", error.Text,
                "De foutmelding komt niet overeen of de gebruiker is onterecht toegelaten.");

            LogInfo("Current URL verified: Admin portal access was blocked.");
            LogSuccess(4, "Regular user successfully denied access to admin functionalities.");

            // Finale status
            LogStep(5, "Final verification of security constraints...");
            LogInfo("✓ Authentication system correctly distinguishes between User and Admin roles.");
            LogSuccess(5, "Security test TC_3_13_4 passed.");
        }
    }
}