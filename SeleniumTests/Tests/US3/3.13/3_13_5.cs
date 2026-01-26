using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US3._13
{
    [TestClass]
    public class _3_13_5 : BaseTest
    {
        [TestMethod]
        public void TC_3_13_5_AdminLogin_WrongPassword()
        {
            const string ADMIN_EMAIL = "admin@example.com";
            const string WRONG_PASSWORD = "verkeerdwachtwoord";

            // Stap 1: Navigatie naar Admin Login
            LogStep(1, "Navigating to the Admin Login page...");
            driver.Navigate().GoToUrl($"{baseUrl}/admin/login");
            wait.Until(d => d.FindElement(By.Id("admin-password")).Displayed);
            LogSuccess(1, "Admin login page loaded.");

            // Stap 2: Invoeren van inloggegevens (Fout wachtwoord)
            LogStep(2, $"Entering valid admin email ({ADMIN_EMAIL}) but incorrect password...");
            driver.FindElement(By.Id("admin-username")).SendKeys(ADMIN_EMAIL);
            driver.FindElement(By.Id("admin-password")).SendKeys(WRONG_PASSWORD);
            LogSuccess(2, "Credentials entered.");

            // Stap 3: Inlogpoging
            LogStep(3, "Submitting admin login form...");
            driver.FindElement(By.Id("admin-login-btn")).Click();
            LogSuccess(3, "Login attempt submitted.");

            // Stap 4: Validatie foutmelding
            LogStep(4, "Waiting for error message and verifying text...");
            var error = wait.Until(d => d.FindElement(By.Id("error-text")));
            LogInfo($"Error message detected: '{error.Text}'");

            Assert.AreEqual("Inloggegevens zijn incorrect", error.Text,
                "De foutmelding komt niet overeen of de toegang werd niet geweigerd.");

            LogInfo("Verification: Admin portal access blocked as expected.");
            LogSuccess(4, "Negative login test passed: Correct error message displayed.");

            // Finale status
            LogStep(5, "Finalizing security check...");
            LogInfo("✓ Unauthorized access with incorrect admin password is successfully prevented.");
            LogSuccess(5, "Test TC_3_13_5 completed successfully.");
        }
    }
}