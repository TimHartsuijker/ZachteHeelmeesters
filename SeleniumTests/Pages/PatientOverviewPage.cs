using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using System.Collections.Generic;

namespace SeleniumTests.Pages
{
    public class PatientOverviewPage(IWebDriver driver): BasePage(driver)
    {
        // URL
        public static string Path => "/patienten";

        // Locators
        private static By PageHeader => By.XPath("//h1[contains(text(), 'Mijn patiënten') or contains(text(), 'Patiënten')]");
        private static By PatientList => By.ClassName("patients-list");
        private static By PatientCards => By.ClassName("patient-row");
        private static By PatientName => By.ClassName("patient-name");
        private static By PatientDetails => By.ClassName("patient-details");
        private static By LoadingIndicator => By.ClassName("loading");
        private static By ErrorMessage => By.CssSelector(".error, .error-message");

        // Actions
        public void Navigate()
        {
            driver.Navigate().GoToUrl(BaseUrl + Path);
        }

        // Verifications
        public bool IsPatientOverviewDisplayed()
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

        public bool IsPatientListDisplayed()
        {
            try
            {
                return driver.FindElement(PatientList).Displayed;
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
                // If loading indicator is found and visible, loading is not complete
                return !driver.FindElement(LoadingIndicator).Displayed;
            }
            catch (NoSuchElementException)
            {
                // If loading indicator doesn't exist, loading is complete
                return true;
            }
        }

        public bool HasPatients()
        {
            try
            {
                var patients = driver.FindElements(PatientCards);
                return patients.Count > 0;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public int GetPatientCount()
        {
            try
            {
                return driver.FindElements(PatientCards).Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        public bool AllPatientsHaveNames()
        {
            try
            {
                var patientCards = driver.FindElements(PatientCards);
                foreach (var card in patientCards)
                {
                    var name = card.FindElement(PatientName);
                    if (string.IsNullOrWhiteSpace(name.Text))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool AllPatientsHaveDateOfBirth()
        {
            try
            {
                var patientCards = driver.FindElements(PatientCards);
                foreach (var card in patientCards)
                {
                    // first detail span is date
                    var details = card.FindElement(PatientDetails);
                    var spans = details.FindElements(By.TagName("span"));
                    if (spans.Count == 0 || string.IsNullOrWhiteSpace(spans[0].Text))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
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

        public List<IWebElement> GetAllPatientCards()
        {
            return driver.FindElements(PatientCards).ToList();
        }
    }
}
