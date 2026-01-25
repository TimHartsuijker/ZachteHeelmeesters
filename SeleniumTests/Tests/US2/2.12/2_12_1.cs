using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US2._12
{
    [TestClass]
    public class _2_12_1 : BaseTest
    {
        [TestMethod]
        public void TC_2_12_1_OpenMedicalDossier()
        {
            const string EMAIL = "gebruiker@example.com";
            const string PASSWORD = "Wachtwoord123";

            // Stap 1: Login en Navigatie
            LogStep(1, $"Logging in as patient ({EMAIL}) and navigating to dossier...");
            helpers.LoginAndNavigateToDossier(EMAIL, PASSWORD);

            Assert.IsTrue(helpers.IsOnDossierPage(),
                $"Patient dossier page did not open. Current URL: {driver.Url}");
            LogSuccess(1, "Successfully reached the patient dossier page.");

            // Stap 2: Laadstatus controleren (Spinner)
            LogStep(2, "Verifying that the loading spinner has disappeared...");
            bool spinnerGone = wait.Until(d => !helpers.IsLoadingSpinnerPresent());

            Assert.IsTrue(spinnerGone, "Loading spinner is still visible after timeout.");
            LogSuccess(2, "Content loading completed.");

            // Stap 3: Data-integriteit (Entries)
            LogStep(3, "Verifying medical dossier entries are loaded...");
            var entries = dossierPage.GetAllEntries();

            Assert.IsTrue(entries.Count > 0, "No dossier entries found - expected at least one record for this test.");
            LogInfo($"Dossier items found: {entries.Count}");
            LogSuccess(3, "Medical records are visible in the dossier view.");

            // Stap 4: URL-verificatie (Sessie-isolatie)
            LogStep(4, "Verifying page URL and session integrity...");
            Assert.IsTrue(driver.Url.Contains("/dossier"), "User is not on the correct dossier sub-route.");
            LogSuccess(4, "Dossier path verified.");

            // Finale audit-log
            LogStep(5, "Final verification of dossier availability...");
            LogInfo("✓ Authentication successful");
            LogInfo("✓ Redirect to /dossier confirmed");
            LogInfo("✓ Data retrieval from backend verified");
            LogSuccess(5, "TC_2_12_1: Medical dossier opened and initialized successfully.");
        }
    }
}