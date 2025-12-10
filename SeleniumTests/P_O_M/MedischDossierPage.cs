using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace SeleniumTests.P_O_M
{
    public class DossierPage
    {
        private readonly IWebDriver driver;

        public DossierPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // URL van de dossierpagina
        public string Url => "http://localhost:5000/dossier"; // Later vervangen

        // ---------- FILTERS ----------
        private By FilterTypeAfspraak => By.XPath("//span[contains(text(),'Afspraak')]");
        private By FilterTypeBehandeling => By.XPath("//span[contains(text(),'Behandeling')]");
        private By DateFromInput => By.XPath("(//input[@type='text'])[1]");
        private By DateToInput => By.XPath("(//input[@type='text'])[2]");
        private By ApplyFilterButton => By.XPath("//button[contains(text(),'Toepassen')]");

        // ---------- ENTRY STRUCTUUR ----------
        // Elke entry begint met datum + toggle
        private By EntryRoot => By.XPath("//div[contains(@class,'entry-root')]");

        // Datum label: '12-11-2024 -'
        private By EntryDate => By.XPath(".//span[contains(@class,'entry-date')]");

        // Toggle per entry
        private By EntryToggleBtn => By.XPath(".//button[contains(@class,'entry-toggle')]");

        // Content-blokken binnen een entry
        private By WordDocumentBlock => By.XPath(".//div[contains(text(),'Word document')]");
        private By NoteBlock => By.XPath(".//div[contains(text(),'notitie')]");
        private By ImageBlock => By.XPath(".//div[contains(text(),'afbeelding')]");

        // Auteur (eindigt met naam, dynamisch)
        private By EntryAuthor => By.XPath(".//span[contains(@class,'entry-author')]");

        // ---------- GENERIEKE METHODS ----------

        public void Navigate()
        {
            driver.Navigate().GoToUrl(Url);
        }

        public void SelectTypeAfspraak()
        {
            driver.FindElement(FilterTypeAfspraak).Click();
        }

        public void SelectTypeBehandeling()
        {
            driver.FindElement(FilterTypeBehandeling).Click();
        }

        public void SetDateFrom(string date)
        {
            var input = driver.FindElement(DateFromInput);
            input.Clear();
            input.SendKeys(date);
        }

        public void SetDateTo(string date)
        {
            var input = driver.FindElement(DateToInput);
            input.Clear();
            input.SendKeys(date);
        }

        public void ClickApplyFilters()
        {
            driver.FindElement(ApplyFilterButton).Click();
        }

        // ---------- ENTRY INTERACTIE ----------

        public IReadOnlyCollection<IWebElement> GetAllEntries()
        {
            return driver.FindElements(EntryRoot);
        }

        public IWebElement GetEntryByDate(string date)
        {
            return GetAllEntries()
                .FirstOrDefault(e => e.FindElement(EntryDate).Text.Contains(date));
        }

        public void ExpandEntry(IWebElement entry)
        {
            entry.FindElement(EntryToggleBtn).Click();
        }

        public bool EntryContainsWordDocument(IWebElement entry)
        {
            return entry.FindElements(WordDocumentBlock).Any();
        }

        public bool EntryContainsCategory(IWebElement entry, string category)
        {
            By blockSelector = category.ToLower() switch
            {
                "word document" => WordDocumentBlock,
                "notitie" => NoteBlock,
                "afbeelding" => ImageBlock,
                _ => throw new KeyNotFoundException($"Categorie '{category}' is onbekend.")
            };

            return entry.FindElements(blockSelector).Any();
        }

        public bool EntryContainsNote(IWebElement entry)
        {
            return entry.FindElements(NoteBlock).Any();
        }

        public bool EntryContainsImage(IWebElement entry)
        {
            return entry.FindElements(ImageBlock).Any();
        }

        public string GetEntryAuthor(IWebElement entry)
        {
            return entry.FindElement(EntryAuthor).Text;
        }

    }
}
