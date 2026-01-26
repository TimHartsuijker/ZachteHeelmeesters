using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeleniumTests.Pages
{
    public class DossierPage(IWebDriver driver): BasePage(driver)
    {
        // URL
        protected override string Path => "/dossier";

        // ---------- FILTERS ----------
        private By TreatmentDropdown => By.CssSelector("select.filter-select");
        private By DateFromInput => By.CssSelector("input[type='date']:first-of-type");
        private By DateToInput => By.CssSelector("input[type='date']:last-of-type");
        private By ApplyFilterButton => By.XPath("//button[contains(text(),'Toepassen')]");

        // ---------- ENTRY STRUCTUUR ----------
        // Entry cards in the dossier
        private By EntryCards => By.CssSelector(".entry-card");

        // Entry header (clickable to expand)
        private By EntryHeader => By.CssSelector(".entry-header");

        // Entry toggle button (chevron)
        private By EntryToggleBtn => By.CssSelector("button.entry-header");

        // File cards within an expanded entry
        private By FileCard => By.CssSelector(".file-card");

        // Notes section
        private By NotesSection => By.XPath(".//div[contains(@class,'bg-gray-50')]//p");

        // Author line
        private By AuthorLine => By.XPath(".//div[contains(@class,'border-t')]//span");

        // ---------- GENERIEKE METHODS ----------

        public void Navigate()
        {
            driver.Navigate().GoToUrl(BaseUrl + Path);
        }

        public void SelectTreatment(string treatmentName)
        {
            var dropdown = driver.FindElement(TreatmentDropdown);
            var selectElement = new SelectElement(dropdown);
            selectElement.SelectByText(treatmentName);
        }

        public void SelectTypeAfspraak()
        {
            // Legacy method for backward compatibility
            SelectTreatment("Afspraak");
        }

        public void SelectTypeBehandeling()
        {
            // Legacy method for backward compatibility
            SelectTreatment("Behandeling");
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
            return driver.FindElements(EntryCards);
        }

        public IWebElement GetEntryByIndex(int index)
        {
            var entries = GetAllEntries();
            return entries.ElementAt(index);
        }

        public void ExpandEntry(IWebElement entry)
        {
            entry.FindElement(EntryToggleBtn).Click();
        }

        public void ToggleEntry(IWebElement entry)
        {
            entry.FindElement(EntryToggleBtn).Click();
        }

        public bool IsEntryExpanded(IWebElement entry)
        {
            // Check if entry has expanded content visible (files, notes, etc.)
            try
            {
                var files = entry.FindElements(FileCard);
                return files.Count > 0 && files[0].Displayed;
            }
            catch
            {
                return false;
            }
        }

        public IReadOnlyCollection<IWebElement> GetFilesInEntry(IWebElement entry)
        {
            return entry.FindElements(FileCard);
        }

        public string GetNotesFromEntry(IWebElement entry)
        {
            try
            {
                var notesElements = entry.FindElements(NotesSection);
                if (notesElements.Count > 0)
                {
                    return notesElements[0].Text;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetAuthorFromEntry(IWebElement entry)
        {
            try
            {
                return entry.FindElement(AuthorLine).Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        public bool EntryContainsFiles(IWebElement entry)
        {
            return entry.FindElements(FileCard).Any();
        }

        public bool EntryContainsWordDocument(IWebElement entry)
        {
            var files = entry.FindElements(FileCard);
            return files.Any(f => f.Text.Contains(".doc") || f.Text.Contains(".docx"));
        }

        public bool EntryContainsCategory(IWebElement entry, string category)
        {
            // Check if entry header or content contains the category/treatment name
            return entry.Text.Contains(category, StringComparison.OrdinalIgnoreCase);
        }

        public bool EntryContainsNote(IWebElement entry)
        {
            return entry.FindElements(NotesSection).Any();
        }

        public bool EntryContainsImage(IWebElement entry)
        {
            var files = entry.FindElements(FileCard);
            return files.Any(f => f.Text.Contains(".jpg") || f.Text.Contains(".png") || f.Text.Contains(".jpeg"));
        }

        public string GetEntryAuthor(IWebElement entry)
        {
            try
            {
                return entry.FindElement(AuthorLine).Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get the date from an entry header text (format: "Title Date")
        /// </summary>
        public string GetEntryDateText(IWebElement entry)
        {
            var headerText = entry.FindElement(EntryHeader).Text;
            // Date is typically in format like "Consult longen - Longgeneeskunde 08-01-2026"
            // Extract the date part (last token)
            var parts = headerText.Split(' ');
            return parts.Length > 0 ? parts[parts.Length - 1] : string.Empty;
        }

        /// <summary>
        /// Check if entries are sorted newest first (by comparing dates in text)
        /// </summary>
        public bool AreEntriesSortedNewestFirst()
        {
            var entries = GetAllEntries();
            if (entries.Count <= 1) return true; // Single or no entries are always sorted

            // Get all dates from entries
            var dates = new List<DateTime>();
            foreach (var entry in entries)
            {
                var dateText = GetEntryDateText(entry);
                // Try to parse date in format dd-MM-yyyy
                if (DateTime.TryParseExact(dateText, "dd-MM-yyyy", 
                    System.Globalization.CultureInfo.InvariantCulture, 
                    System.Globalization.DateTimeStyles.None, 
                    out DateTime date))
                {
                    dates.Add(date);
                }
            }

            // Check if dates are in descending order (newest first)
            for (int i = 0; i < dates.Count - 1; i++)
            {
                if (dates[i] < dates[i + 1])
                {
                    return false; // Found older date before newer date
                }
            }

            return true;
        }

    }
}
