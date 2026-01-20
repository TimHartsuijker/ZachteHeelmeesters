using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests.P_O_M
{
    public class PatientOverviewPage(IWebDriver driver)
    {
        private readonly IWebDriver driver = driver;

        // URL
        public static string Url => "http://localhost:5173/patienten";

        // Locators
        private static By PageHeader => By.XPath("//h1[contains(text(), 'Patiëntenoverzicht') or contains(text(), 'Patiënten')]");
        private static By PatientList => By.ClassName("patient-list");
        private static By PatientCards => By.ClassName("patient-card");
        private static By PatientName => By.ClassName("patient-name");
        private static By PatientDateOfBirth => By.ClassName("patient-dob");
        private static By LoadingIndicator => By.ClassName("loading");
        private static By ErrorMessage => By.CssSelector(".error, .error-message");

        // Actions
        public void Navigate()
        {
            driver.Navigate().GoToUrl(Url);
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
                    var dob = card.FindElement(PatientDateOfBirth);
                    if (string.IsNullOrWhiteSpace(dob.Text))
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

        public IReadOnlyCollection<IWebElement> GetAllPatientCards()
        {
            return driver.FindElements(PatientCards);
        }
    }
}
