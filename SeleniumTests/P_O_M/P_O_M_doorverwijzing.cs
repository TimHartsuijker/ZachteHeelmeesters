using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests.Pages
{
    public class DoorverwijzingPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        

        public DoorverwijzingPage(IWebDriver driver)
        {
            this._driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public string Url => "http://localhost:5000/doorverwijzing"; // Later vervangen

        private By PatientInput => By.Id("patient");
        private By TreatmentInput => By.Id("treatment");
        private By NoteInput => By.Id("note");
        private By ConfirmButton => By.Id("confirm-btn");

        public void Navigate()
        {
            _driver.Navigate().GoToUrl(Url);
        }

        // Wait for page to load and return true if all required elements are displayed, false on timeout
        public bool WaitForPageLoad()
        {
            try
            {
                return _wait.Until(d =>
                {
                    try
                    {
                        var patient = d.FindElement(PatientInput);
                        var treatment = d.FindElement(TreatmentInput);
                        var note = d.FindElement(NoteInput);
                        var confirm = d.FindElement(ConfirmButton);

                        return patient.Displayed && treatment.Displayed && note.Displayed && confirm.Displayed;
                    }
                    catch (NoSuchElementException)
                    {
                        return false;
                    }
                });
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        // Try to detect that a referral was created.
        // NOTE: update selectors/text to match your application's success indicator if necessary.
        public bool IsReferralCreated(int timeoutSeconds = 5)
        {
            try
            {
                var shortWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutSeconds));
                return shortWait.Until(d =>
                {
                    try
                    {
                        // Common success indicators: visible success alerts/toasts or a URL change
                        var successSelectors = new[]
                        {
                            By.CssSelector(".alert-success"),
                            By.CssSelector(".toast-success"),
                            By.XPath("//*[contains(translate(text(),'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'),'doorverwijzing')]")
                        };

                        foreach (var sel in successSelectors)
                        {
                            var elems = d.FindElements(sel);
                            if (elems.Any(e => e.Displayed))
                                return true;
                        }

                        // If the app redirects after creation, URL will change from the form URL
                        if (!string.IsNullOrEmpty(d.Url) && !d.Url.Equals(Url, System.StringComparison.OrdinalIgnoreCase))
                            return true;

                        return false;
                    }
                    catch (NoSuchElementException)
                    {
                        return false;
                    }
                });
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public void EnterPatient(string patient)
        {
            var input = _driver.FindElement(PatientInput);
            input.Clear();
            input.SendKeys(patient);
        }

        public void EnterTreatment(string treatment)
        {
            var input = _driver.FindElement(TreatmentInput);
            input.Clear();
            input.SendKeys(treatment);
        }

        public void NotePatient(string note)
        {
            var input = _driver.FindElement(NoteInput);
            input.Clear();
            input.SendKeys(note);
        }

        public void ClickConfirm()
        {
            _driver.FindElement(By.XPath("//button[contains(text(), 'doorverwijzing aanmaken')]")).Click();
        }

        public bool IsPatientInputDisplayed()
        {
            return _driver.FindElement(PatientInput).Displayed;
        }

        public bool IsTreatmentInputDisplayed()
        {
            return _driver.FindElement(TreatmentInput).Displayed;
        }

        public bool IsNoteInputDisplayed()
        {
            return _driver.FindElement(NoteInput).Displayed;
        }

        public bool IsConfirmButtonDisplayed()
        {
            return _driver.FindElement(ConfirmButton).Displayed;
        }

    }
}