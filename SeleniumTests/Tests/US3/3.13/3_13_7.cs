using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US3._13
{
    [TestClass]
    public class _3_13_7 : BaseTest
    {
        [TestMethod]
        public void TC_3_13_7_AdminLogin_EmptyFields()
        {
            // Stap 1: Navigatie naar Admin Login
            LogStep(1, "Navigating to the Admin Login page...");
            adminLoginPage.Navigate();
            wait.Until(d => adminLoginPage.IsPasswordInputDisplayed());
            LogSuccess(1, "Admin login page loaded.");

            // Stap 2: Inloggen met lege velden
            LogStep(2, "Attempting to login with both email and password fields empty...");
            adminLoginPage.PerformLogin("", "");
            LogSuccess(2, "Empty login attempt submitted.");

            // Stap 3: Validatie van de foutmelding
            LogStep(3, "Waiting for validation message and verifying text...");
            var errorText = wait.Until(d => adminLoginPage.WaitForError());
            LogInfo($"Error message detected: '{errorText}'");

            Assert.AreEqual("Gegevens moeten ingevuld zijn", errorText,
                "De validatiemelding voor lege velden is niet correct.");

            LogInfo("Verification: System correctly prevented submission with empty credentials.");
            LogSuccess(3, "Validation test passed: Mandatory field check is operational.");

            // Finale status
            LogStep(4, "Finalizing security validation...");
            LogInfo("✓ Empty field validation on the admin portal is successfully verified.");
            LogSuccess(4, "Test TC_3_13_7 completed successfully.");
        }
    }
}