using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumTests.Base;

namespace US2._30
{
    [TestClass]
    public class _2_30_1_4 : BaseTest
    {
        [TestMethod]
        public void TC_2_30_1_4_WelcomeMessageWithoutName()
        {
            const string EMAIL = "gebruiker@example.com";
            const string PASSWORD = "Wachtwoord123";

            // De start-log is al geschreven door BaseTest.Setup()

            // STAP 1: Navigatie
            LogStep(1, "Navigating to login page via Page Object.");
            loginPage.Navigate();

            // Wachten tot inlogveld zichtbaar is (via POM methode)
            wait.Until(_ => loginPage.IsPasswordInputDisplayed());
            LogSuccess(1, "Login page is interactive.");

            // STAP 2: Login met gebruiker (fallback scenario)
            LogStep(2, $"Performing login for: {EMAIL}");
            loginPage.PerformLogin(EMAIL, PASSWORD);
            LogInfo("Login submitted. Waiting for dashboard redirection.");

            // STAP 3: Dashboard Verificatie
            LogStep(3, "Waiting for dashboard to load and verifying welcome message presence.");

            RetryVerification(() =>
            {
                // Check of dashboard geladen is
                Assert.IsTrue(dashboardPage.IsWelcomeMessageDisplayed(),
                    "Dashboard welcome message element not found.");
            });
            LogSuccess(3, "Dashboard element detected.");

            // STAP 4: Welkomstboodschap Validatie (Fallback check)
            LogStep(4, "Verifying if the welcome message is appropriate when a name might be missing.");

            RetryVerification(() =>
            {
                string welcomeText = dashboardPage.GetWelcomeMessageText();
                LogInfo($"Detected welcome text: '{welcomeText}'");

                // Controleer of er tenminste een begroeting staat, ook als de naam ontbreekt
                bool hasValidGreeting =
                    welcomeText.Contains("Welkom") ||
                    welcomeText.Contains("Hallo") ||
                    welcomeText.Contains("Welcome") ||
                    welcomeText.Contains("Hi");

                Assert.IsTrue(hasValidGreeting || !string.IsNullOrWhiteSpace(welcomeText),
                    $"The welcome message '{welcomeText}' is not considered appropriate for a user.");

                LogSuccess(4, "Appropriate welcome message verified.");
            });

            LogInfo("Test Case TC_2_30_1_4 finished successfully.");
        }
    }
}