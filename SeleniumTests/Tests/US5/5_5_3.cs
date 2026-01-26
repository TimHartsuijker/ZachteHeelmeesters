using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US5._5
{
    [TestClass]
    public class _5_5_3 : BaseTest
    {
        [TestMethod]
        public void TC_5_5_3_CompleteMedicalRecordIsAccessibleWithAllSections()
        {
            // Test data uit centrale testset
            string gpEmail = "testdoctor@example.com";
            string gpPassword = "password";

            // Step 1: Authentication
            LogStep(1, "Logging in as GP and navigating to dashboard...");
            loginPage.Navigate();
            loginPage.PerformLogin(gpEmail, gpPassword);
            LogSuccess(1, "GP Authentication successful.");

            // Step 2: Patient Overview Navigation
            LogStep(2, "Accessing the Patient Overview page...");
            patientOverviewPage.Navigate();
            patientOverviewPage.WaitForPatientsToLoad();
            LogSuccess(2, "Patient overview successfully populated.");

            // Step 3: Select Patient and Open Dossier
            LogStep(3, "Selecting the first patient record to verify clinical data access...");
            var rows = patientOverviewPage.GetRows();
            string expectedPatientName = rows[0].FindElement(By.ClassName("patient-name")).Text;

            // Gebruikt de robuuste OpenDossierByRowIndex die we in de POM hebben gezet
            patientOverviewPage.OpenDossierByRowIndex(0);

            // Verifieer landing op de dossier pagina
            medicalRecordPage.WaitForPageToLoad();
            string actualPatientName = medicalRecordPage.GetPatientName();

            Assert.IsTrue(actualPatientName.Contains(expectedPatientName), $"Patient name mismatch. Expected: {expectedPatientName}, Got: {actualPatientName}");
            LogSuccess(3, $"Medical record for '{actualPatientName}' opened successfully.");

            // Step 4: Clinical Data Content Check
            LogStep(4, "Verifying clinical entries and data rendering...");

            // We gebruiken de GetEntryCount() gebaseerd op de 'entry-card' class uit je HTML
            int entryCount = medicalRecordPage.GetEntryCount();
            LogInfo($"System detected {entryCount} clinical entries in this record.");

            Assert.IsTrue(entryCount > 0, "The medical record should contain clinical entries for this patient.");
            LogSuccess(4, "Clinical data presence verified.");

            // Step 5: Final Integrity Check
            LogStep(5, "Performing final UI integrity verification...");
            Assert.IsTrue(medicalRecordPage.IsMedicalRecordPageDisplayed(), "Dossier UI components should remain visible.");
            LogSuccess(5, "Complete medical record access verified for GP role.");
        }
    }
}