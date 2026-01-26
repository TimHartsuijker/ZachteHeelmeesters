using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US5._5
{
    [TestClass]
    public class _5_5_1 : BaseTest
    {
        [TestMethod]
        public void TC_5_5_1_HuisartsCanViewAllAssignedPatientsWithBasicInfo()
        {
            // Test data
            string gpEmail = "testdoctor@example.com";
            string gpPassword = "password";

            // Step 1: Login
            LogStep(1, "Authenticating as GP...");
            loginPage.Navigate();
            loginPage.PerformLogin(gpEmail, gpPassword);
            LogSuccess(1, "Login submitted.");

            // Step 2: Navigation
            LogStep(2, "Navigating to Patient Overview...");
            patientOverviewPage.Navigate();

            // Stabiliteit: Wacht tot de pagina en de data er zijn
            patientOverviewPage.IsPatientOverviewDisplayed();
            patientOverviewPage.WaitForPatientsToLoad();
            LogSuccess(2, "Patient overview loaded with data.");

            // Step 3: Multi-layered Data Integrity Check
            LogStep(3, "Verifying data integrity for all rendered patient records...");

            // We gebruiken de Retry hier omdat data soms asynchroon binnendruppelt
            RetryVerification(() =>
            {
                Assert.IsTrue(patientOverviewPage.HasPatients(), "No patients found in the list.");
                Assert.IsTrue(patientOverviewPage.VerifyDataIntegrityForAllPatients(),
                    "Data integrity check failed: one or more records are incomplete.");
            }, maxAttempts: 3, delayMs: 2000);

            int count = patientOverviewPage.GetPatientCount();
            LogInfo($"Total patients verified: {count}");
            LogSuccess(3, "All patient records contain valid Name, DOB, and Contact info.");

            // Step 4: Detailed evidence logging
            LogStep(4, "Logging specific patient details for the test report...");
            var rows = patientOverviewPage.GetRows();
            for (int i = 0; i < rows.Count; i++)
            {
                var name = rows[i].FindElement(By.ClassName("patient-name")).Text;
                LogInfo($"Verified Record {i + 1}: {name}");
            }
            LogSuccess(4, "Evidence logging completed.");
        }
    }
}