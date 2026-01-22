using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;

namespace SeleniumTests.Tests.US5
{
    /// <summary>
    /// Test-ID: TC5.5-1
    /// Test name: Patiëntenoverzicht Huisarts
    /// Description: Controleer of de huisarts een overzicht van alle toegewezen patiënten 
    /// kan openen en of per patiënt de basisinformatie correct wordt weergegeven.
    /// User Story: US5.5 - Als huisarts wil ik altijd het medisch dossier van een patiënt 
    /// kunnen inzien, zodat ik volledige informatie heb voor het verlenen van zorg.
    /// </summary>
    [TestClass]
    public class _5_5_1
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "http://localhost:5173";

        private LoginPage loginPage;
        private PatientOverviewPage patientOverviewPage;

        [TestInitialize]
        public void Setup()
        {
            Console.WriteLine("=== Test Setup: TC5.5-1 - Patiëntenoverzicht Huisarts ===");
            
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            loginPage = new LoginPage(driver);
            patientOverviewPage = new PatientOverviewPage(driver);
            
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
        public void TC_5_5_1_HuisartsCanViewAllAssignedPatientsWithBasicInfo()
        {
            Console.WriteLine("\n*** Starting Test: TC_5_5_1_HuisartsCanViewAllAssignedPatientsWithBasicInfo ***\n");

            // Test credentials for GP (huisarts)
            string gpEmail = "testdoctor@example.com";
            string gpPassword = "password";

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

            // Step 2: Navigate to patient overview
            Console.WriteLine("[Step 3] Waiting for successful login and redirecting to patient overview...");
            
            // Wait for redirect after login - could be dashboard or directly to patients page
            wait.Until(d => !d.Url.Contains("/inloggen") && !d.Url.Contains("/login"));
            Console.WriteLine($"[Step 3] Redirected to: {driver.Url}");

            // Navigate to patient overview page
            Console.WriteLine("[Step 4] Navigating to patient overview page...");
            patientOverviewPage.Navigate();
            
            // Wait for page to load
            wait.Until(d => patientOverviewPage.IsPatientOverviewDisplayed());
            Console.WriteLine("[Step 4] ✓ Patient overview page loaded successfully.");

            // Step 3: View the list of patients and verify content
            Console.WriteLine("[Step 5] Verifying patient overview is fully loaded...");
            
            // Wait for loading to complete
            wait.Until(d => patientOverviewPage.IsLoadingComplete());
            Console.WriteLine("[Step 5] ✓ Page loading complete.");

            // Verify no error messages are displayed
            Console.WriteLine("[Step 6] Checking for error messages...");
            Assert.IsFalse(patientOverviewPage.HasErrorMessage(), 
                $"No error messages should be displayed. Error: {patientOverviewPage.GetErrorMessage()}");
            Console.WriteLine("[Step 6] ✓ No error messages found.");

            // Verify patient list is displayed
            Console.WriteLine("[Step 7] Verifying patient list is displayed...");
            Assert.IsTrue(patientOverviewPage.IsPatientListDisplayed(), 
                "Patient list should be displayed");
            Console.WriteLine("[Step 7] ✓ Patient list is visible.");

            // Expected Result: Overview shows all assigned patients
            Console.WriteLine("[Step 8] Verifying patients are displayed...");
            Assert.IsTrue(patientOverviewPage.HasPatients(), 
                "Overview should show all assigned patients");
            
            int patientCount = patientOverviewPage.GetPatientCount();
            Console.WriteLine($"[Step 8] ✓ Found {patientCount} patient(s) in the overview.");
            Assert.IsTrue(patientCount > 0, 
                "At least one patient should be displayed for the GP");

            // Expected Result: Per patient, name and date of birth are displayed
            Console.WriteLine("[Step 9] Verifying all patients have required basic information...");
            
            Assert.IsTrue(patientOverviewPage.AllPatientsHaveNames(), 
                "All patients should have their names displayed");
            Console.WriteLine("[Step 9a] ✓ All patients have names displayed.");

            Assert.IsTrue(patientOverviewPage.AllPatientsHaveDateOfBirth(), 
                "All patients should have their date of birth displayed");
            Console.WriteLine("[Step 9b] ✓ All patients have date of birth displayed.");

            // Additional verification: Log patient details for manual verification
            Console.WriteLine("\n[Step 10] Patient details found:");
            var patientCards = patientOverviewPage.GetAllPatientCards();
            int index = 1;
            foreach (var card in patientCards)
            {
                try
                {
                    var name = card.FindElement(By.ClassName("patient-name")).Text;
                    var dob = card.FindElement(By.ClassName("patient-dob")).Text;
                    Console.WriteLine($"  Patient {index}: Name='{name}', DOB='{dob}'");
                    index++;
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine($"  Patient {index}: Could not retrieve complete information");
                    index++;
                }
            }

            // Expected Result: Overview is clear and fully loaded
            Console.WriteLine("\n[Step 11] Final verification - overview is clear and complete...");
            Assert.IsTrue(patientOverviewPage.IsPatientOverviewDisplayed(), 
                "Patient overview should remain displayed");
            Assert.IsTrue(patientOverviewPage.IsLoadingComplete(), 
                "All content should be fully loaded");
            Console.WriteLine("[Step 11] ✓ Patient overview is clear and fully loaded.");

            Console.WriteLine("\n*** TEST PASSED: TC_5_5_1 - GP can view all assigned patients with basic information ***");
        }
    }
}
