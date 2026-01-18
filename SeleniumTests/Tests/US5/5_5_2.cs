using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;

namespace SeleniumTests.Tests.US5
{
    /// <summary>
    /// Test-ID: TC5.5-2
    /// Test name: Open Meerdere Patiëntendossiers
    /// Description: Verifieer dat de huisarts vanuit het patiëntenoverzicht meerdere 
    /// patiëntendossiers kan openen zonder beperkingen.
    /// User Story: US5.5 - Als huisarts wil ik altijd het medisch dossier van een patiënt 
    /// kunnen inzien, zodat ik volledige informatie heb voor het verlenen van zorg.
    /// </summary>
    [TestClass]
    public class _5_5_2
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "http://localhost:5173";

        private LoginPage loginPage;
        private PatientOverviewPage patientOverviewPage;
        private PatientMedicalRecordPage medicalRecordPage;

        [TestInitialize]
        public void Setup()
        {
            Console.WriteLine("=== Test Setup: TC5.5-2 - Open Meerdere Patiëntendossiers ===");
            
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            loginPage = new LoginPage(driver);
            patientOverviewPage = new PatientOverviewPage(driver);
            medicalRecordPage = new PatientMedicalRecordPage(driver);
            
            Console.WriteLine("WebDriver initialized successfully.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
                Console.WriteLine("WebDriver closed and disposed.");
            }
            Console.WriteLine("=== Test Cleanup Complete ===\n");
        }

        [TestMethod]
        public void TC_5_5_2_HuisartsCanOpenMultiplePatientRecordsWithoutRestrictions()
        {
            Console.WriteLine("\n*** Starting Test: TC_5_5_2_HuisartsCanOpenMultiplePatientRecordsWithoutRestrictions ***\n");

            // Test credentials for GP (huisarts)
            string gpEmail = "huisarts@example.com";
            string gpPassword = "Wachtwoord123";

            // Step 1: Log in as GP (huisarts)
            Console.WriteLine("[Step 1] Navigating to login page...");
            loginPage.Navigate();
            Assert.IsTrue(loginPage.IsLoginPageDisplayed(), 
                "Login page should be displayed");
            Console.WriteLine("[Step 1] ✓ Login page loaded successfully.");

            Console.WriteLine($"[Step 2] Logging in as GP with email: {gpEmail}");
            loginPage.EnterEmail(gpEmail);
            loginPage.EnterPassword(gpPassword);
            loginPage.ClickLogin();
            Console.WriteLine("[Step 2] ✓ Login credentials submitted.");

            // Step 2: Open patient overview
            Console.WriteLine("[Step 3] Waiting for successful login...");
            wait.Until(d => !d.Url.Contains("/inloggen") && !d.Url.Contains("/login"));
            Console.WriteLine($"[Step 3] Logged in successfully, redirected to: {driver.Url}");

            Console.WriteLine("[Step 4] Navigating to patient overview...");
            patientOverviewPage.Navigate();
            wait.Until(d => patientOverviewPage.IsPatientOverviewDisplayed());
            wait.Until(d => patientOverviewPage.IsLoadingComplete());
            Console.WriteLine("[Step 4] ✓ Patient overview page loaded.");

            // Verify at least 2 patients are available
            Console.WriteLine("[Step 5] Verifying at least 2 patients are available...");
            Assert.IsTrue(patientOverviewPage.HasPatients(), 
                "Patient overview should contain patients");
            
            int patientCount = patientOverviewPage.GetPatientCount();
            Console.WriteLine($"[Step 5] Found {patientCount} patient(s).");
            Assert.IsTrue(patientCount >= 2, 
                $"At least 2 patients required for this test. Found: {patientCount}");
            Console.WriteLine("[Step 5] ✓ Sufficient patients available for test.");

            // Get patient cards
            var patientCards = patientOverviewPage.GetAllPatientCards();
            IWebElement patientACard = patientCards[0];
            IWebElement patientBCard = patientCards[1];

            // Store patient names for verification
            string patientAName = patientACard.FindElement(By.ClassName("patient-name")).Text;
            string patientBName = patientBCard.FindElement(By.ClassName("patient-name")).Text;
            Console.WriteLine($"[Step 5] Identified Patient A: '{patientAName}'");
            Console.WriteLine($"[Step 5] Identified Patient B: '{patientBName}'");

            // Step 3: Open the medical record of Patient A
            Console.WriteLine($"\n[Step 6] Opening medical record for Patient A: '{patientAName}'...");
            patientACard.Click();

            // Wait for medical record page to load
            wait.Until(d => medicalRecordPage.IsMedicalRecordPageDisplayed());
            wait.Until(d => medicalRecordPage.IsLoadingComplete());
            Console.WriteLine("[Step 6] ✓ Medical record page loaded.");

            // Expected Result: Medical record opens without errors
            Console.WriteLine("[Step 7] Verifying Patient A's medical record opened successfully...");
            Assert.IsFalse(medicalRecordPage.HasErrorMessage(), 
                $"No error messages should be displayed. Error: {medicalRecordPage.GetErrorMessage()}");
            Console.WriteLine("[Step 7a] ✓ No error messages displayed.");

            // Expected Result: No permission required
            Assert.IsFalse(medicalRecordPage.HasPermissionError(), 
                "No permission error should be displayed - GP should have unrestricted access");
            Console.WriteLine("[Step 7b] ✓ No permission required - GP has unrestricted access.");

            // Expected Result: Correct patient data is displayed
            Assert.IsTrue(medicalRecordPage.IsPatientInfoDisplayed(), 
                "Patient information should be displayed");
            Assert.IsTrue(medicalRecordPage.HasPatientName(), 
                "Patient name should be displayed");
            
            string displayedPatientAName = medicalRecordPage.GetPatientName();
            Assert.IsTrue(displayedPatientAName.Contains(patientAName) || patientAName.Contains(displayedPatientAName), 
                $"Correct patient data should be displayed. Expected: '{patientAName}', Got: '{displayedPatientAName}'");
            Console.WriteLine($"[Step 7c] ✓ Correct patient data displayed: '{displayedPatientAName}'");

            Assert.IsTrue(medicalRecordPage.IsMedicalRecordContentDisplayed(), 
                "Medical record content should be displayed");
            Console.WriteLine("[Step 7d] ✓ Medical record content is displayed.");

            // Step 4: Go back to overview
            Console.WriteLine("\n[Step 8] Navigating back to patient overview...");
            
            // Try to click back button, or navigate directly if button not available
            if (medicalRecordPage.IsBackButtonDisplayed())
            {
                medicalRecordPage.ClickBackButton();
                Console.WriteLine("[Step 8] ✓ Clicked back button.");
            }
            else
            {
                patientOverviewPage.Navigate();
                Console.WriteLine("[Step 8] ✓ Navigated directly to overview.");
            }

            wait.Until(d => patientOverviewPage.IsPatientOverviewDisplayed());
            wait.Until(d => patientOverviewPage.IsLoadingComplete());
            Console.WriteLine("[Step 8] ✓ Back at patient overview page.");

            // Verify we're back at the overview
            Assert.IsTrue(patientOverviewPage.HasPatients(), 
                "Patient list should be displayed again");
            Console.WriteLine("[Step 8] ✓ Patient list is displayed.");

            // Step 5: Open the medical record of Patient B
            Console.WriteLine($"\n[Step 9] Opening medical record for Patient B: '{patientBName}'...");
            
            // Re-fetch patient cards as DOM might have changed
            var refreshedPatientCards = patientOverviewPage.GetAllPatientCards();
            IWebElement refreshedPatientBCard = refreshedPatientCards[1];
            refreshedPatientBCard.Click();

            // Wait for medical record page to load
            wait.Until(d => medicalRecordPage.IsMedicalRecordPageDisplayed());
            wait.Until(d => medicalRecordPage.IsLoadingComplete());
            Console.WriteLine("[Step 9] ✓ Medical record page loaded.");

            // Expected Result: Medical record opens without errors
            Console.WriteLine("[Step 10] Verifying Patient B's medical record opened successfully...");
            Assert.IsFalse(medicalRecordPage.HasErrorMessage(), 
                $"No error messages should be displayed. Error: {medicalRecordPage.GetErrorMessage()}");
            Console.WriteLine("[Step 10a] ✓ No error messages displayed.");

            // Expected Result: No permission required
            Assert.IsFalse(medicalRecordPage.HasPermissionError(), 
                "No permission error should be displayed - GP should have unrestricted access");
            Console.WriteLine("[Step 10b] ✓ No permission required - GP has unrestricted access.");

            // Expected Result: Correct patient data is displayed for Patient B
            Assert.IsTrue(medicalRecordPage.IsPatientInfoDisplayed(), 
                "Patient information should be displayed");
            Assert.IsTrue(medicalRecordPage.HasPatientName(), 
                "Patient name should be displayed");
            
            string displayedPatientBName = medicalRecordPage.GetPatientName();
            Assert.IsTrue(displayedPatientBName.Contains(patientBName) || patientBName.Contains(displayedPatientBName), 
                $"Correct patient data should be displayed. Expected: '{patientBName}', Got: '{displayedPatientBName}'");
            Console.WriteLine($"[Step 10c] ✓ Correct patient data displayed: '{displayedPatientBName}'");

            // Verify it's a different patient
            Assert.AreNotEqual(displayedPatientAName, displayedPatientBName, 
                "Patient B's name should be different from Patient A's name");
            Console.WriteLine($"[Step 10d] ✓ Confirmed different patient (A: '{displayedPatientAName}' vs B: '{displayedPatientBName}')");

            Assert.IsTrue(medicalRecordPage.IsMedicalRecordContentDisplayed(), 
                "Medical record content should be displayed");
            Console.WriteLine("[Step 10e] ✓ Medical record content is displayed.");

            // Final verification
            Console.WriteLine("\n[Step 11] Final verification...");
            Console.WriteLine("✓ Both medical records opened without errors");
            Console.WriteLine("✓ No patient permission required");
            Console.WriteLine("✓ Each record displayed correct patient data");
            Console.WriteLine("✓ GP has unrestricted access to all patient medical records");

            Console.WriteLine("\n*** TEST PASSED: TC_5_5_2 - GP can open multiple patient records without restrictions ***");
        }
    }
}
