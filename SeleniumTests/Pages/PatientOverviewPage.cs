using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using System.Collections.Generic;

namespace SeleniumTests.Pages
{
    public class PatientOverviewPage(IWebDriver driver) : BasePage(driver)
    {
        protected override string Path => "/patienten";

        // Precise Locators based on your HTML
        private static By PageHeader => By.TagName("h1");
        private static By PatientRows => By.ClassName("patient-row");
        private static By PatientNameLabel => By.ClassName("patient-name");
        private static By PatientDetailItems => By.ClassName("detail-item");
        private static By ViewDossierButton => By.ClassName("btn-view-record");

        public void Navigate() => NavigateTo(BaseUrl + Path);

        public bool IsHeaderCorrect() => GetText(PageHeader) == "Mijn patiënten";

        public List<IWebElement> GetRows() => Driver.FindElements(PatientRows).ToList();

        public string GetNameByRow(IWebElement row)
        {
            return row.FindElement(By.ClassName("patient-name")).Text.Trim();
        }

        /// <summary>
        /// Validates that every patient row has a name, email, and a working dossier button.
        /// </summary>
        public bool VerifyDataIntegrityForAllPatients()
        {
            var rows = GetRows();
            if (rows.Count == 0) return false;

            return rows.All(row =>
            {
                bool hasName = !string.IsNullOrWhiteSpace(row.FindElement(PatientNameLabel).Text);
                bool hasDetails = row.FindElements(PatientDetailItems).Count >= 2;
                bool hasButton = row.FindElement(ViewDossierButton).Displayed;
                return hasName && hasDetails && hasButton;
            });
        }

        public int GetPatientCount() => GetRows().Count;

        public bool HasPatients() => GetPatientCount() > 0;

        /// <summary>
        /// Clicks the "Dossier Openen" button for a specific patient by their name.
        /// </summary>
        public void OpenDossierForPatient(string name)
        {
            var targetRow = GetRows().FirstOrDefault(r =>
                r.FindElement(PatientNameLabel).Text.Trim().Equals(name, System.StringComparison.OrdinalIgnoreCase)) 
                ?? throw new NoSuchElementException($"Patient with name '{name}' not found in the list.");

            targetRow.FindElement(ViewDossierButton).Click();
        }
        public void OpenDossierByRowIndex(int index)
        {
            var rows = GetRows();
            if (index >= rows.Count) throw new IndexOutOfRangeException($"Geen patiënt gevonden op index {index}");

            var dossierButton = rows[index].FindElement(ViewDossierButton);

            // Gebruik de robuuste klik-logica uit je test, maar nu centraal in de POM
            try
            {
                dossierButton.Click();
            }
            catch
            {
                // Fallback naar JavaScript klik als de UI blokkeert
                ExecuteJs("arguments[0].click();", dossierButton);
            }
        }

        public bool IsPatientOverviewDisplayed()
        {
            try
            {
                // Wacht tot de header aanwezig en zichtbaar is
                WaitForElement(PageHeader);
                return Driver.FindElement(PageHeader).Displayed;
            }
            catch { return false; }
        }

        public void WaitForPatientsToLoad()
        {
            try
            {
                // Wacht tot de lijst aanwezig is, maar wees specifiek over wat er gebeurt
                Wait.Until(d => {
                    var rows = d.FindElements(PatientRows);
                    if (rows.Count > 0) return true;

                    // Check of er misschien een "Geen patiënten gevonden" melding staat
                    var emptyMsg = d.FindElements(By.ClassName("no-patients-feedback"));
                    if (emptyMsg.Count > 0) throw new Exception("API returned 0 patients for this GP.");

                    return false;
                });
            }
            catch (WebDriverTimeoutException)
            {
                throw;
            }
        }

        public bool IsPatientListDisplayed() => IsElementDisplayed(PatientRows);
    }
}