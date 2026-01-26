using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumTests.Base;

namespace US2._30
{
    [TestClass]
    public class _2_30_2_2 : BaseTest
    {
        [TestMethod]
        public void NavigationMenu_ContainsCorrectOptions()
        {
            const string EMAIL = "gebruiker@example.com";
            const string PASSWORD = "Wachtwoord123";

            // Stap 1: Login via POM
            LogStep(1, "Navigating to login page and entering credentials.");
            loginPage.Navigate();
            loginPage.PerformLogin(EMAIL, PASSWORD);
            LogInfo("Login submitted.");

            // Stap 2: Wachten op Dashboard via POM verificatie
            LogStep(2, "Waiting for Dashboard to load.");
            RetryVerification(() =>
            {
                Assert.IsTrue(dashboardPage.IsWelcomeMessageDisplayed(), "Dashboard failed to load: Welcome message not found.");
            });
            LogSuccess(2, "Dashboard loaded successfully.");

            // Stap 3: Functionaliteit Verifiëren via POM
            LogStep(3, "Verifying presence and titles of dashboard panels.");

            RetryVerification(() =>
            {
                // Controleer zichtbaarheid van componenten via de DashboardPage POM
                Assert.IsTrue(dashboardPage.IsDashboardGridDisplayed(), "Dashboard grid is not visible.");

                // Haal titels op via POM methodes
                string leftTitle = dashboardPage.GetLeftPanelTitle();
                string rightTitle = dashboardPage.GetRightPanelTitle();

                LogInfo($"Checking titles - Left: '{leftTitle}', Right: '{rightTitle}'");

                // Controleer op correcte opties (NL/EN support)
                bool hasAppointments = leftTitle.Contains("Afspraken") || leftTitle.Contains("Appointments");
                bool hasReferrals = rightTitle.Contains("Doorverwijzingen") || rightTitle.Contains("Referrals");

                Assert.IsTrue(hasAppointments, $"Left panel does not contain expected 'Appointments' title. Found: {leftTitle}");
                Assert.IsTrue(hasReferrals, $"Right panel does not contain expected 'Referrals' title. Found: {rightTitle}");

                LogSuccess(3, $"Verified correct options: {leftTitle} and {rightTitle}.");
            });
        }
    }
}