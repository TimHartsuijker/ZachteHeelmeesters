using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US3._13
{
    [TestClass]
    public class _3_13_6 : BaseTest
    {
        [TestMethod]
        public void TC_3_13_6_AdminLogin_UnknownCredentials()
        {
            const string FAKE_EMAIL = "fakeadmin@example.com";
            const string FAKE_PASSWORD = "EnigWachtwoord123";

            // Stap 1: Navigatie naar Admin Login
            LogStep(1, "Navigating to the Admin Login page...");
            driver.Navigate().GoToUrl($"{baseUrl}/admin/login");
            wait.Until(d => d.FindElement(By.Id("admin-username")).Displayed);
            LogSuccess(1, "Admin login page loaded.");

            // Stap 2: Invoeren van onbekende credentials
            LogStep(2, $"Entering unknown credentials (Email: {FAKE_EMAIL})...");
            driver.FindElement(By.Id("admin-username")).SendKeys(FAKE_EMAIL);
            driver.FindElement(By.Id("admin-password")).SendKeys(FAKE_PASSWORD);
            LogSuccess(2, "Unknown credentials entered.");

            // Stap 3: Inlogpoging
            LogStep(3, "Submitting admin login form...");
            driver.FindElement(By.Id("admin-login-btn")).Click();
            LogSuccess(3, "Login attempt submitted.");

            // Stap 4: Validatie foutmelding
            LogStep(4, "Waiting for error message and verifying refusal...");
            var error = wait.Until(d => d.FindElement(By.Id("error-text")));
            LogInfo($"Error message detected: '{error.Text}'");

            Assert.AreEqual("Inloggegevens zijn incorrect", error.Text,
                "Het systeem gaf niet de verwachte foutmelding bij onbekende inloggegevens.");

            LogInfo("Verification: System correctly denied access to non-existent account.");
            LogSuccess(4, "Access denied as expected. Security check passed.");

            // Finale status
            LogStep(5, "Finalizing security validation...");
            LogInfo("✓ Unauthorized access with unknown credentials is successfully blocked.");
            LogSuccess(5, "Test TC_3_13_6 completed successfully.");
        }
    }
}