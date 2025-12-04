using OpenQA.Selenium;

namespace SeleniumTests.P_O_M
{
    public class MedischDossierPage
    {
        private readonly IWebDriver driver;

        public MedischDossierPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // ---------------------------
        // URL van het medisch dossier
        // ---------------------------
        public string Url => "http://localhost:5000/medisch-dossier"; // aanpassen indien nodig


        // ---------------------------
        // Locators (alle elementen)
        // ---------------------------

        // AC2.12.1 - Knop om het medisch dossier te openen
        private By BtnOpenMedicalRecord => By.Id("open-medical-record-btn");

        // AC2.12.2 - Medische geschiedenis entries
        private By MedicalHistoryEntries => By.CssSelector(".medical-history-entry");

        // AC2.12.3 - Filter sectie
        private By FilterComponent => By.Id("filter-component");
        private By FilterDateInput => By.Id("filter-date");
        private By FilterCategoryInput => By.Id("filter-category");
        private By ApplyFilterButton => By.Id("apply-filter");

        // AC2.12.4 - Mag niet zichtbaar zijn
        private By EditButtons => By.CssSelector(".edit-button");


        // ---------------------------
        // Navigatie
        // ---------------------------

        public void Navigate()
        {
            driver.Navigate().GoToUrl(Url);
        }


        // ---------------------------
        // Acties
        // ---------------------------

        // AC2.12.1 - Dossier openen
        public void OpenMedicalRecord()
        {
            driver.FindElement(BtnOpenMedicalRecord).Click();
        }

        // Filteren van medische geschiedenis
        public void FilterByDate(string date)
        {
            var dateInput = driver.FindElement(FilterDateInput);
            dateInput.Clear();
            dateInput.SendKeys(date);

            driver.FindElement(ApplyFilterButton).Click();
        }

        public void FilterByCategory(string category)
        {
            var categoryInput = driver.FindElement(FilterCategoryInput);
            categoryInput.Clear();
            categoryInput.SendKeys(category);

            driver.FindElement(ApplyFilterButton).Click();
        }

        public bool IsMedicalHistoryVisible()
        {
            return driver.FindElements(MedicalHistoryEntries).Count > 0;
        }

        public int GetMedicalHistoryCount()
        {
            return driver.FindElements(MedicalHistoryEntries).Count;
        }

        public bool AreEditButtonsVisible()
        {
            return driver.FindElements(EditButtons).Any(e => e.Displayed);
        }

        public bool IsFilterComponentDisplayed()
        {
            return driver.FindElement(FilterComponent).Displayed;
        }
    }
}
