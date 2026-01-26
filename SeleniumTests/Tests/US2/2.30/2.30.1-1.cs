using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumTests.Base;

namespace US2._30
{
    [TestClass]
    public class _2_30_1_1 : BaseTest
    {
        [TestMethod]
        public void TC_2_30_1_1_Dashboard_DisplaysWelcomeMessage()
        {
            // Gebruik variabelen voor logging/data integriteit
            const string EMAIL = "gebruiker@example.com";
            const string PASSWORD = "Wachtwoord123";
            const string EXPECTED_NAME = "Test Gebruiker";

            // STAP 1: Navigatie
            LogStep(1, "Navigating to the login page.");
            loginPage.Navigate();

            // Wachten op de pagina (gebruik de wait uit BaseTest)
            wait.Until(_ => loginPage.IsPasswordInputDisplayed());
            LogSuccess(1, "Login page loaded successfully.");

            // STAP 2: Login Actie
            LogStep(2, $"Attempting login with user: {EMAIL}");
            loginPage.PerformLogin(EMAIL, PASSWORD);
            LogInfo("Login credentials submitted.");

            // STAP 3: Dashboard Verificatie
            LogStep(3, "Verifying dashboard content and welcome message visibility.");

            // Gebruik de RetryVerification uit BaseTest tegen 'flaky' UI momenten
            RetryVerification(() =>
            {
                // 1. Controleer URL
                Assert.IsTrue(dashboardPage.IsUrlCorrect("/dashboard"),
                    "Redirection to /dashboard failed.");

                // 2. Controleer of bericht getoond wordt
                Assert.IsTrue(dashboardPage.IsWelcomeMessageDisplayed(),
                    "Welcome message element not found on dashboard.");

                // 3. Controleer de inhoud
                string actualText = dashboardPage.GetWelcomeMessageText();
                StringAssert.Contains(actualText, EXPECTED_NAME,
                    $"Dashboard message does not contain expected user name. Found: '{actualText}'");

                LogSuccess(3, $"Dashboard verified. Message found: '{actualText}'");
            });
        }
    }
}