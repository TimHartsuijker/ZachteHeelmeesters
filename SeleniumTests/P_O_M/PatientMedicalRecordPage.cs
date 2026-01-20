using OpenQA.Selenium;

namespace SeleniumTests.P_O_M
{
    public class PatientMedicalRecordPage(IWebDriver driver)
    {
        private readonly IWebDriver driver = driver;

        // URL pattern - actual URL will be /patienten/{patientId}/dossier or similar
        public static string GetUrl(string patientId) => $"http://localhost:5173/patienten/{patientId}/dossier";

        // Locators
        private static By PageHeader => By.XPath("//h1[contains(text(), 'Medisch Dossier') or contains(text(), 'Dossier')]");
        private static By PatientNameHeader => By.ClassName("patient-name-header");
        private static By PatientInfo => By.ClassName("patient-info");
        private static By PatientDateOfBirth => By.ClassName("patient-dob");
        private static By MedicalRecordContent => By.ClassName("medical-record-content");
        private static By MedicalRecordEntries => By.ClassName("medical-record-entry");
        private static By BackButton => By.XPath("//button[contains(text(), 'Terug') or contains(@class, 'back-button')]");
        private static By LoadingIndicator => By.ClassName("loading");
        private static By ErrorMessage => By.CssSelector(".error, .error-message");
        private static By PermissionError => By.XPath("//*[contains(text(), 'toestemming') or contains(text(), 'niet toegestaan')]");
        
        // Medical record sections
        private static By ComplaintsSection => By.XPath("//section[contains(@class, 'complaints') or //h2[contains(text(), 'Klachten')]/following-sibling::*]");
        private static By DiagnosesSection => By.XPath("//section[contains(@class, 'diagnoses') or //h2[contains(text(), 'Diagnoses')]/following-sibling::*]");
        private static By TreatmentsSection => By.XPath("//section[contains(@class, 'treatments') or //h2[contains(text(), 'Behandelingen')]/following-sibling::*]");
        private static By ReferralsSection => By.XPath("//section[contains(@class, 'referrals') or //h2[contains(text(), 'Doorverwijzingen')]/following-sibling::*]");
        
        // Section headers (alternative locators)
        private static By ComplaintsSectionHeader => By.XPath("//h2[contains(text(), 'Klachten')] | //h3[contains(text(), 'Klachten')]");
        private static By DiagnosesSectionHeader => By.XPath("//h2[contains(text(), 'Diagnoses')] | //h3[contains(text(), 'Diagnoses')]");
        private static By TreatmentsSectionHeader => By.XPath("//h2[contains(text(), 'Behandelingen')] | //h3[contains(text(), 'Behandelingen')]");
        private static By ReferralsSectionHeader => By.XPath("//h2[contains(text(), 'Doorverwijzingen')] | //h3[contains(text(), 'Doorverwijzingen')]");
        
        // Generic section items
        private static By SectionItems => By.XPath("//*[contains(@class, 'item') or contains(@class, 'entry')]");
        private static By SearchBox => By.XPath("//input[@placeholder='Zoeken' or @placeholder='Search' or contains(@class, 'search')]");

        // Actions
        public void Navigate(string patientId)
        {
            driver.Navigate().GoToUrl(GetUrl(patientId));
        }

        public void ClickBackButton()
        {
            driver.FindElement(BackButton).Click();
        }

        // Verifications
        public bool IsMedicalRecordPageDisplayed()
        {
            try
            {
                return driver.FindElement(PageHeader).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsPatientInfoDisplayed()
        {
            try
            {
                return driver.FindElement(PatientInfo).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool HasPatientName()
        {
            try
            {
                var nameElement = driver.FindElement(PatientNameHeader);
                return !string.IsNullOrWhiteSpace(nameElement.Text);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public string GetPatientName()
        {
            try
            {
                return driver.FindElement(PatientNameHeader).Text;
            }
            catch (NoSuchElementException)
            {
                return string.Empty;
            }
        }

        public bool HasPatientDateOfBirth()
        {
            try
            {
                var dobElement = driver.FindElement(PatientDateOfBirth);
                return !string.IsNullOrWhiteSpace(dobElement.Text);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsMedicalRecordContentDisplayed()
        {
            try
            {
                return driver.FindElement(MedicalRecordContent).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsLoadingComplete()
        {
            try
            {
                return !driver.FindElement(LoadingIndicator).Displayed;
            }
            catch (NoSuchElementException)
            {
                return true;
            }
        }

        public bool HasErrorMessage()
        {
            try
            {
                var errorElements = driver.FindElements(ErrorMessage);
                return errorElements.Count > 0 && errorElements[0].Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public string GetErrorMessage()
        {
            try
            {
                return driver.FindElement(ErrorMessage).Text;
            }
            catch (NoSuchElementException)
            {
                return string.Empty;
            }
        }

        public bool HasPermissionError()
        {
            try
            {
                var permissionErrors = driver.FindElements(PermissionError);
                return permissionErrors.Count > 0 && permissionErrors[0].Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsBackButtonDisplayed()
        {
            try
            {
                return driver.FindElement(BackButton).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public int GetMedicalRecordEntriesCount()
        {
            try
            {
                return driver.FindElements(MedicalRecordEntries).Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        // Medical record section verification methods
        public bool HasComplaintsSection()
        {
            try
            {
                var headers = driver.FindElements(ComplaintsSectionHeader);
                return headers.Count > 0 && headers[0].Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool HasDiagnosesSection()
        {
            try
            {
                var headers = driver.FindElements(DiagnosesSectionHeader);
                return headers.Count > 0 && headers[0].Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool HasTreatmentsSection()
        {
            try
            {
                var headers = driver.FindElements(TreatmentsSectionHeader);
                return headers.Count > 0 && headers[0].Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool HasReferralsSection()
        {
            try
            {
                var headers = driver.FindElements(ReferralsSectionHeader);
                return headers.Count > 0 && headers[0].Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public int GetComplaintsCount()
        {
            try
            {
                var header = driver.FindElement(ComplaintsSectionHeader);
                var parent = header.FindElement(By.XPath(".."));
                return parent.FindElements(By.XPath(".//*[contains(@class, 'item') or contains(@class, 'entry')]")).Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        public int GetDiagnosesCount()
        {
            try
            {
                var header = driver.FindElement(DiagnosesSectionHeader);
                var parent = header.FindElement(By.XPath(".."));
                return parent.FindElements(By.XPath(".//*[contains(@class, 'item') or contains(@class, 'entry')]")).Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        public int GetTreatmentsCount()
        {
            try
            {
                var header = driver.FindElement(TreatmentsSectionHeader);
                var parent = header.FindElement(By.XPath(".."));
                return parent.FindElements(By.XPath(".//*[contains(@class, 'item') or contains(@class, 'entry')]")).Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        public int GetReferralsCount()
        {
            try
            {
                var header = driver.FindElement(ReferralsSectionHeader);
                var parent = header.FindElement(By.XPath(".."));
                return parent.FindElements(By.XPath(".//*[contains(@class, 'item') or contains(@class, 'entry')]")).Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        public bool IsSearchFunctionalityAvailable()
        {
            try
            {
                return driver.FindElement(SearchBox).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void SearchInMedicalRecord(string searchTerm)
        {
            try
            {
                var searchField = driver.FindElement(SearchBox);
                searchField.Clear();
                searchField.SendKeys(searchTerm);
            }
            catch (NoSuchElementException)
            {
                throw new Exception("Search box not found in medical record");
            }
        }

        public bool AllSectionsAccessible()
        {
            return HasComplaintsSection() && HasDiagnosesSection() && 
                   HasTreatmentsSection() && HasReferralsSection();
        }

        public bool AllSectionsContainData()
        {
            return GetComplaintsCount() > 0 && GetDiagnosesCount() > 0 && 
                   GetTreatmentsCount() > 0 && GetReferralsCount() > 0;
        }
