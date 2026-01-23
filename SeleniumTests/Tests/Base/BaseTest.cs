using backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using System;
using System.IO;
using System.Linq;

namespace SeleniumTests.Base
{
    [TestClass] // Nodig voor AssemblyCleanup support
    public abstract class BaseTest
    {
        private static int _totalTests = 0;
        private static int _totalPassed = 0;

        public TestContext TestContext { get; set; } = null!;

        protected IWebDriver driver { get; private set; } = null!;
        protected WebDriverWait wait { get; private set; } = null!;
        protected readonly string baseUrl = "http://localhost";

        protected string downloadDirectory = null!;
        protected bool useDownloadConfig = false;

        protected AppDbContext _context = null!;
        protected bool useDbContext = false;

        protected static string? _logFilePath;
        private static readonly object _fileLock = new object();

        // Page Objects
        protected LoginPage loginPage { get; private set; } = null!;
        protected DashboardPage dashboardPage { get; private set; } = null!;
        protected AdminLoginPage adminLoginPage { get; private set; } = null!;
        protected UserManagementPage userManagementPage { get; private set; } = null!;
        protected RegistrationPage registrationPage { get; private set; } = null!;
        protected DossierPage dossierPage { get; private set; } = null!;
        protected TestHelpers helpers { get; private set; } = null!;
        protected PatientOverviewPage patientOverviewPage { get; private set; } = null!;
        protected PatientMedicalRecordPage medicalRecordPage { get; private set; } = null!;

        [TestInitialize]
        public virtual void Setup()
        {
            // Initialiseer logbestand bij de eerste test
            GetLogFilePath();

            LogInfo($"=== Starting Test: {TestContext.TestName} ===");

            if (useDbContext)
            {
                ContextSetup();
            }

            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            if (useDownloadConfig)
            {
                downloadDirectory = Path.Combine(Path.GetTempPath(), "SeleniumDownloads_" + Guid.NewGuid().ToString());
                Directory.CreateDirectory(downloadDirectory);
                LogInfo($"Download directory created: {downloadDirectory}");

                options.AddUserProfilePreference("download.default_directory", downloadDirectory);
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddUserProfilePreference("download.directory_upgrade", true);
                options.AddUserProfilePreference("safebrowsing.enabled", true);
            }

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Page Initializatie
            loginPage = new LoginPage(driver);
            dashboardPage = new DashboardPage(driver);
            adminLoginPage = new AdminLoginPage(driver);
            userManagementPage = new UserManagementPage(driver);
            registrationPage = new RegistrationPage(driver);
            dossierPage = new DossierPage(driver);
            helpers = new TestHelpers(driver, loginPage, dossierPage);
            patientOverviewPage = new PatientOverviewPage(driver);
            medicalRecordPage = new PatientMedicalRecordPage(driver);
        }

        #region Logging Engine
        protected void LogStep(int step, string message) => WriteToLog($"[STEP {step}] {message}");
        protected void LogSuccess(int step, string message) => WriteToLog($"[SUCCESS {step}] ✓ {message}");
        protected void LogInfo(string message) => WriteToLog($"[INFO] {message}");

        private void WriteToLog(string message)
        {
            string formattedMsg = $"{DateTime.Now:HH:mm:ss} - {message}";
            Console.WriteLine(formattedMsg);

            try
            {
                string path = GetLogFilePath();
                lock (_fileLock)
                {
                    File.AppendAllText(path, formattedMsg + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij schrijven naar logfile: {ex.Message}");
            }
        }

        private string GetLogFilePath()
        {
            if (_logFilePath == null)
            {
                lock (_fileLock)
                {
                    if (_logFilePath == null)
                    {
                        string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestRuns");
                        if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

                        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        _logFilePath = Path.Combine(logDir, $"TestRun_{timestamp}.log");

                        File.WriteAllText(_logFilePath, $"=== TEST SESSION STARTED: {DateTime.Now} ==={Environment.NewLine}{Environment.NewLine}");
                    }
                }
            }
            return _logFilePath;
        }
        #endregion

        #region Database Context
        public void ContextSetup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=ZachteHeelmeesters_Dev;User=sa;Password=ZHM_Dev_Password1!;TrustServerCertificate=True;Encrypt=False")
                .Options;

            _context = new AppDbContext(options);
            ResetDatabase();
        }

        private void ResetDatabase()
        {
            LogInfo("Resetting database and applying seed data...");
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            DbSeederStatic.Seed(_context);
            DbSeederTest.Seed(_context);
            DbSeederMedicalFiles.Seed(_context);
            LogInfo("Database reset complete.");
        }
        #endregion

        [TestCleanup]
        public virtual void Cleanup()
        {
            _totalTests++;
            bool passed = TestContext.CurrentTestOutcome == UnitTestOutcome.Passed;

            if (passed) _totalPassed++;

            string resultSymbol = passed ? "PASS" : "FAIL";
            LogInfo($"*** TEST {resultSymbol}: {TestContext.TestName} ***{Environment.NewLine}");

            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }

            _context?.Dispose();

            // Verwijder temp download directory indien nodig
            if (useDownloadConfig && Directory.Exists(downloadDirectory))
            {
                try { Directory.Delete(downloadDirectory, true); } catch { }
            }
        }

        [AssemblyCleanup]
        public static void GlobalCleanup()
        {
            if (_logFilePath != null && File.Exists(_logFilePath))
            {
                lock (_fileLock)
                {
                    string summary = Environment.NewLine +
                                     "===============================================" + Environment.NewLine +
                                     $"=== TEST RUN FINISHED: {DateTime.Now} ===" + Environment.NewLine +
                                     $"Total Tests Run: {_totalTests}" + Environment.NewLine +
                                     $"Total Passed:    {_totalPassed}" + Environment.NewLine +
                                     $"Total Failed:    {_totalTests - _totalPassed}" + Environment.NewLine +
                                     "===============================================";

                    File.AppendAllText(_logFilePath, summary);
                }
            }
        }
    }
}