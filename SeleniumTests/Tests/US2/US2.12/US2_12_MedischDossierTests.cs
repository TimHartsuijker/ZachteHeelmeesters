using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.P_O_M;
using System.Diagnostics;

namespace SeleniumTests.Tests
{
    [TestClass]
    public class MedischDossierTests
    {
        private static IWebDriver driver;
        private static LoginPage loginPage;
        private static DossierPage dossierPage;

        private void Log(string message)
        {
            Trace.WriteLine($"[LOG] {message}");
            Console.WriteLine($"[LOG] {message}");
        }

        public static class TestData
        {
            public static string PatientEmail => "patient@example.com";
            public static string PatientPassword => "Test123!";
            public static string PatientName => "Jan Jansen";
            public static string ChronoFrom => "01-01-2025";
            public static string ChronoTo => "01-06-2025";
            public static int ChronoExpectedCount => 2;
            public static string InvalidFrom => "01-12-2025";
            public static string InvalidTo => "01-01-2025";
            public static string CombinedFrom => "01-06-2025";
            public static string CombinedTo => "01-12-2025";
            public static string CombinedCategory => "Afspraak";
            public static string CategoryFilter => "Behandeling";

            // ---------------------- BACKEND FAILURE FLAGS ------------------
            public static string BackendDownFlag => "?backendDown=true";
            public static string HistoryFailFlag => "?historyFail=true";

            // ---------------------- READ-ONLY CHECKS -----------------------
            public static string[] ForbiddenEditKeywords = new[]
            {
                "bewerken",
                "wijzig",
                "verwijderen",
                "<input",
                "<textarea"
            };
        }

        // =====================================================================
        // SETUP / CLEANUP
        // =====================================================================
        [ClassInitialize]
        public static void TestSetup(TestContext context)
        {
            driver = new ChromeDriver();

            loginPage = new LoginPage(driver);
            dossierPage = new DossierPage(driver);
        }

        [ClassCleanup]
        public static void TestTeardown()
        {
            driver.Quit();
        }

        // =====================================================================
        // AC2.12.1 Open Medical Record
        // =====================================================================

        [TestMethod]
        public void TC_2_12_1_1_SuccessfullyOpenMedicalRecord()
        {
            loginPage.LoginAsPatient(TestData.PatientEmail, TestData.PatientPassword);
            dossierPage.Navigate();

            var entries = dossierPage.GetAllEntries();
            Assert.IsTrue(entries.Count > 0, "Medisch dossier zou minimaal één entry moeten tonen.");
            Log("Assert passed: er is minimaal één entry in het medisch dossier.");
        }

        [TestMethod]
        public void TC_2_12_1_3_BackendFailureShowsError()
        {
            driver.Navigate().GoToUrl(dossierPage.Url + TestData.BackendDownFlag);

            bool containsError = driver.PageSource.Contains("kan niet geladen worden") ||
                                 driver.PageSource.Contains("Probeer het later opnieuw");

            Assert.IsTrue(containsError, "Foutmelding ontbreekt bij backend failure.");
            Log("Assert passed: foutmelding correct weergegeven bij backend failure.");
        }

        // =====================================================================
        // AC2.12.2 View medical history
        // =====================================================================

        [TestMethod]
        public void TC_2_12_2_1_CompleteHistoryIsVisible()
        {
            loginPage.LoginAsPatient(TestData.PatientEmail, TestData.PatientPassword);
            dossierPage.Navigate();

            var entries = dossierPage.GetAllEntries();
            Assert.IsTrue(entries.Count >= 1, "De volledige medische geschiedenis zou zichtbaar moeten zijn.");
            Log("Assert passed: volledige medische geschiedenis zichtbaar.");
        }

        [TestMethod]
        public void TC_2_12_2_7_LoadingHistoryFailureShowsError()
        {
            loginPage.LoginAsPatient(TestData.PatientEmail, TestData.PatientPassword);
            driver.Navigate().GoToUrl(dossierPage.Url + TestData.HistoryFailFlag);

            bool containsError = driver.PageSource.Contains("kon niet geladen worden") ||
                                 driver.PageSource.Contains("Probeer het later opnieuw");

            Assert.IsTrue(containsError, "Foutmelding ontbreekt bij mislukte geschiedenis-loading.");
            Log("Assert passed: foutmelding correct weergegeven bij geschiedenis-loading failure.");
        }


        // =====================================================================
        // AC2.12.3 Data filtering
        // =====================================================================

        [TestMethod]
        public void TC_2_12_3_1_FilterChronological()
        {
            loginPage.LoginAsPatient(TestData.PatientEmail, TestData.PatientPassword);
            dossierPage.Navigate();

            dossierPage.SetDateFrom(TestData.ChronoFrom);
            dossierPage.SetDateTo(TestData.ChronoTo);
            dossierPage.ClickApplyFilters();

            var entries = dossierPage.GetAllEntries();

            Assert.AreEqual(TestData.ChronoExpectedCount, entries.Count,
                "Chronologische filtering levert een onverwacht aantal entries op.");
            Log("Assert passed: correcte resultaten voor chronologische filtering.");
        }

        [TestMethod]
        public void TC_2_12_3_2_FilterByCategory()
        {
            loginPage.LoginAsPatient(TestData.PatientEmail, TestData.PatientPassword);
            dossierPage.Navigate();

            // Selecteer filter knop gebaseerd op category
            if (TestData.CategoryFilter == "Afspraak")
                dossierPage.SelectTypeAfspraak();
            else if (TestData.CategoryFilter == "Behandeling")
                dossierPage.SelectTypeBehandeling();

            dossierPage.ClickApplyFilters();

            var entries = dossierPage.GetAllEntries();

            // Controleer dat elke entry het juiste category content block bevat
            Assert.IsTrue(entries.All(e => dossierPage.EntryContainsCategory(e, TestData.CategoryFilter)),
                $"Alle entries moeten categorie '{TestData.CategoryFilter}' hebben.");

            Log("Assert passed: alle entries bevatten de juiste categorie.");
        }

        [TestMethod]
        public void TC_2_12_3_4_InvalidDateRangeShowsValidation()
        {
            loginPage.LoginAsPatient(TestData.PatientEmail, TestData.PatientPassword);
            dossierPage.Navigate();

            dossierPage.SetDateFrom(TestData.InvalidFrom);
            dossierPage.SetDateTo(TestData.InvalidTo);
            dossierPage.ClickApplyFilters();

            bool hasValidation = driver.PageSource.Contains("ongeldige datum") ||
                                  driver.PageSource.Contains("validatie");

            Assert.IsTrue(hasValidation, "Ongeldig datumbereik zou een validatiefout moeten tonen.");
            Log("Assert passed: validatiefout correct weergegeven bij ongeldig datumbereik.");
        }

        [TestMethod]
        public void TC_2_12_3_5_CombinedFilteringWorks()
        {
            loginPage.LoginAsPatient(TestData.PatientEmail, TestData.PatientPassword);
            dossierPage.Navigate();

            if (TestData.CombinedCategory == "Afspraak") dossierPage.SelectTypeAfspraak();
            else if (TestData.CombinedCategory == "Behandeling") dossierPage.SelectTypeBehandeling();

            dossierPage.SetDateFrom(TestData.CombinedFrom);
            dossierPage.SetDateTo(TestData.CombinedTo);
            dossierPage.ClickApplyFilters();

            var entries = dossierPage.GetAllEntries();

            Assert.IsTrue(entries.Count > 0, "Gecombineerde filtering zou resultaten moeten opleveren.");
            Log("Assert passed: gecombineerde filtering heeft resultaten.");

            Assert.IsTrue(entries.All(e => dossierPage.EntryContainsCategory(e, TestData.CombinedCategory)),
                "Gecombineerde filtering bevat elementen met een verkeerde categorie.");

            Log("Assert passed: gecombineerde filtering toont correcte categorie.");
        }


        // =====================================================================
        // AC2.12.4 Read-only
        // =====================================================================

        [TestMethod]
        public void TC_2_12_4_1_NoModificationAllowed()
        {
            loginPage.LoginAsPatient(TestData.PatientEmail, TestData.PatientPassword);
            dossierPage.Navigate();

            var html = driver.PageSource.ToLower();

            foreach (var keyword in TestData.ForbiddenEditKeywords)
            {
                Assert.IsFalse(html.Contains(keyword), $"Het dossier bevat een bewerkbaar element: '{keyword}'.");
                Log($"Assert passed: geen bewerkbaar element gevonden voor keyword '{keyword}'.");
            }
        }
    }
}
