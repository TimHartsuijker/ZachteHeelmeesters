using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumTests.Base;

namespace US2._30
{
    [TestClass]
    public class _2_30_2_1 : BaseTest
    {
        [TestMethod]
        public void NavigationMenu_VisibleAfterLogin()
        {
            const string EMAIL = "gebruiker@example.com";
            const string PASSWORD = "Wachtwoord123";

            // STAP 1 & 2: Navigatie en Login via POM
            LogStep(1, "Navigating to login page and performing login.");
            loginPage.Navigate();

            // Wachten tot het veld interactief is (gebruikt de 'wait' van BaseTest)
            wait.Until(_ => loginPage.IsPasswordInputDisplayed());

            loginPage.PerformLogin(EMAIL, PASSWORD);
            LogSuccess(1, "Login credentials submitted successfully.");

            // STAP 3: Dashboard laden
            LogStep(2, "Waiting for dashboard elements to be visible.");
            RetryVerification(() =>
            {
                Assert.IsTrue(dashboardPage.IsWelcomeMessageDisplayed(), "Dashboard failed to load: Welcome message not visible.");
            });
            LogSuccess(2, "Dashboard successfully loaded.");

            // STAP 4: Verificatie van navigatie-elementen (Panels)
            LogStep(3, "Verifying visibility of dashboard grid and functional panels.");

            RetryVerification(() =>
            {
                // Verifieer Grid
                Assert.IsTrue(dashboardPage.IsDashboardGridDisplayed(), "Dashboard grid is not visible.");

                // Verifieer de Navbar (die je in de POM hebt toegevoegd)
                Assert.IsTrue(dashboardPage.IsNavbarVisible(), "Top navigation bar is not visible.");

                // Verifieer Panels via de titels (dit bewijst dat ze niet alleen bestaan, maar ook data bevatten)
                string leftTitle = dashboardPage.GetLeftPanelTitle();
                string rightTitle = dashboardPage.GetRightPanelTitle();
                string welcomeText = dashboardPage.GetWelcomeMessageText();

                LogInfo($"Dashboard Content: Welcome='{welcomeText}', Left='{leftTitle}', Right='{rightTitle}'");

                Assert.IsFalse(string.IsNullOrWhiteSpace(leftTitle), "Left panel title is empty.");
                Assert.IsFalse(string.IsNullOrWhiteSpace(rightTitle), "Right panel title is empty.");

                LogSuccess(3, "All dashboard navigation elements (Navbar, Grid, Panels) are visible and populated.");
            });
        }
    }
}