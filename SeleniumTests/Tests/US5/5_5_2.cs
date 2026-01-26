using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US5._5
{
    [TestClass]
    public class _5_5_2 : BaseTest
    {
        [TestMethod]
        public void TC_5_5_2_HuisartsCanOpenMultiplePatientRecordsWithoutRestrictions()
        {
            string gpEmail = "testdoctor@example.com";
            string gpPassword = "password";

            // Step 1 & 2: Login & Navigation
            LogStep(1, "Logging in as GP...");
            loginPage.Navigate();
            loginPage.PerformLogin(gpEmail, gpPassword);

            LogStep(2, "Navigating to patient overview...");
            patientOverviewPage.Navigate();
            patientOverviewPage.WaitForPatientsToLoad();

            // Step 3: Identify Patients
            LogStep(3, "Identifying patients for sequential test...");
            var rows = patientOverviewPage.GetRows();
            string nameA = patientOverviewPage.GetNameByRow(rows[0]);
            string nameB = patientOverviewPage.GetNameByRow(rows[1]);

            // Step 4: Access Patient A
            LogStep(4, $"Opening record for Patient A: {nameA}");
            patientOverviewPage.OpenDossierByRowIndex(0);

            // De POM handelt nu het wachten af
            bool nameAVerified = medicalRecordPage.WaitForPatientName(nameA);
            Assert.IsTrue(nameAVerified, $"Header name mismatch for Patient A. Found: {medicalRecordPage.GetPatientName()}");
            LogSuccess(4, "Record A verified.");

            // Step 5: Return and Access Patient B
            LogStep(5, "Returning to overview and opening Patient B...");
            patientOverviewPage.Navigate();
            patientOverviewPage.WaitForPatientsToLoad();

            patientOverviewPage.OpenDossierByRowIndex(1);

            // De POM handelt wederom het wachten af
            bool nameBVerified = medicalRecordPage.WaitForPatientName(nameB);
            string displayedNameB = medicalRecordPage.GetPatientName();

            Assert.IsTrue(nameBVerified, $"Header name mismatch for Patient B. Found: {displayedNameB}");
            Assert.AreNotEqual(nameA, displayedNameB, "Security Alert: UI showed data leakage from Patient A!");
            LogSuccess(5, "Record B verified. State-transition integrity confirmed.");
        }
    }
}