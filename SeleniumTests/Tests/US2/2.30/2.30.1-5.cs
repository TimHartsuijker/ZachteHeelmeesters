using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumTests.Base;

namespace US2._30
{
    [TestClass]
    public class _2_30_1_5 : BaseTest
    {
        [TestMethod]
        public void TC_2_30_1_5_Dashboard_NameMatchesDatabase()
        {
            // Data configuratie
            const string EMAIL = "gebruiker@example.com";
            const string PASSWORD = "Wachtwoord123";
            const string EXPECTED_NAME = "Test Gebruiker";

            // STAP 1: Navigatie
            LogStep(1, "Navigating to login page.");
            loginPage.Navigate();

            // Wachten op input veld (via POM methode)
            wait.Until(_ => loginPage.IsPasswordInputDisplayed());
            LogSuccess(1, "Login page is ready.");

            // STAP 2: Login
            LogStep(2, $"Logging in with credentials for: {EMAIL}");
            loginPage.PerformLogin(EMAIL, PASSWORD);
            LogInfo("Credentials submitted.");

            // STAP 3: Dashboard laden
            LogStep(3, "Waiting for dashboard to load.");
            RetryVerification(() =>
            {
                Assert.IsTrue(dashboardPage.IsWelcomeMessageDisplayed(),
                    "Dashboard welcome message not visible after login.");
            });
            LogSuccess(3, "Dashboard loaded successfully.");

            // STAP 4: Data Verificatie
            LogStep(4, $"Verifying if welcome message contains database name: '{EXPECTED_NAME}'");

            RetryVerification(() =>
            {
                string actualMessage = dashboardPage.GetWelcomeMessageText();
                LogInfo($"Found message text: '{actualMessage}'");

                Assert.IsTrue(actualMessage.Contains(EXPECTED_NAME),
                    $"Database name mismatch. Expected: '{EXPECTED_NAME}' | Actual: '{actualMessage}'");

                LogSuccess(4, $"Name verification passed: '{actualMessage}' matches expected data.");
            });
        }
    }
}