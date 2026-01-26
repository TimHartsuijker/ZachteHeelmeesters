using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using System.Collections.Generic;

namespace SeleniumTests.Pages
{
    public class MedicalRecordPage(IWebDriver driver) : BasePage(driver)
    {
        protected override string Path => "/dossier/";

        // Locators based on provided HTML
        private static By PatientNameHeader => By.CssSelector("h2.text-lg.font-semibold");
        private static By BackButton => By.ClassName("back-button");
        private static By EntryCards => By.ClassName("entry-card");
        private static By EntryHeader => By.ClassName("entry-header");

        // Filter Locators
        private static By TreatmentSelect => By.ClassName("filter-select");
        private static By DateInputs => By.ClassName("filter-input");
        private static By ApplyFilterButton => By.ClassName("action-btn");

        // Verifications
        public void WaitForPageToLoad() => WaitForElement(PatientNameHeader);

        public string GetPatientName() => GetText(PatientNameHeader);

        /// <summary>
        /// Wacht tot de header de specifieke naam van de patiënt bevat.
        /// Dit lost de 'Race Condition' op waarbij de oude naam nog zichtbaar is.
        /// </summary>
        public bool WaitForPatientName(string expectedName)
        {
            return Wait.Until(d =>
                GetPatientName().Contains(expectedName, System.StringComparison.OrdinalIgnoreCase));
        }

        public bool IsMedicalRecordPageDisplayed() => IsElementDisplayed(PatientNameHeader);

        public List<IWebElement> GetMedicalEntries() => Driver.FindElements(EntryCards).ToList();

        public int GetEntryCount() => GetMedicalEntries().Count;

        // Actions
        public void ClickBackToOverview() => Click(BackButton);

        public void FilterByTreatment(string treatmentValue)
        {
            SelectByValue(TreatmentSelect, treatmentValue);
            Click(ApplyFilterButton);
            // Wait for potential UI refresh
            System.Threading.Thread.Sleep(500);
        }

        public bool AllEntriesContainText(string text)
        {
            return GetMedicalEntries().All(entry =>
                entry.FindElement(EntryHeader).Text.Contains(text, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}