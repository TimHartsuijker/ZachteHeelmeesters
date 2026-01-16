using OpenQA.Selenium;

namespace SeleniumTests.P_O_M
{
    public class RegistrationPage
    {
        private readonly IWebDriver driver;

        public RegistrationPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public static string Url => "http://localhost:5173/register";

        private static By FirstNameInput => By.Id("firstname");
        private static By LastNameInput => By.Id("lastname");
        private static By DateOfBirthInput => By.Id("dateofbirth");
        private static By GenderInput => By.Id("gender");
        private static By StreetnameInput => By.Id("streetname");
        private static By HousenumberInput => By.Id("housenumber");
        private static By PostalcodeInput => By.Id("postalcode");
        private static By CitizenServiceNumberInput => By.Id("csn");
        private static By PhoneNumberInput => By.Id("phonenumber");
        private static By EmailInput => By.Id("email");
        private static By PasswordInput => By.Id("password");
        private static By RegisterButton => By.Id("register-btn");
        private static By ErrorMessage => By.Id("error-message");

        public void Navigate()
        {
            Console.WriteLine("Navigate to the registrationpage...");
            driver.Navigate().GoToUrl(Url);
        }

        public void EnterData(By locator, string value)
        {
            string locatorValue = locator.ToString().Split(':')[1].Trim();
            Console.WriteLine($"Entering {locatorValue}: {value}");

            var input = driver.FindElement(locator);
            input.Clear();
            input.SendKeys(value);
        }

        public void Register(string firstname, string lastname, string streetname, string housenumber, string postalcode, string citizenservicenumber, string dateofbirth, string gender, string phone_number, string email, string password)
        {
            Navigate();

            if (!IsAllDisplayed())
            {
                throw new NoSuchElementException("One or more registration fields are not displayed.");
            }
            
            Console.WriteLine("Registrationpage loaded.");
            
            EnterData(FirstNameInput, firstname);
            EnterData(LastNameInput, lastname);
            EnterData(EmailInput, email);
            EnterData(PasswordInput, password);
            EnterData(StreetnameInput, streetname);
            EnterData(HousenumberInput, housenumber);
            EnterData(PostalcodeInput, postalcode);
            EnterData(CitizenServiceNumberInput, citizenservicenumber);
            EnterData(DateOfBirthInput, dateofbirth);
            EnterData(GenderInput, gender);
            EnterData(PhoneNumberInput, phone_number);

            Console.WriteLine("Clicking the register button");
            driver.FindElement(RegisterButton).Click();
        }

        public bool IsDisplayed(By locator)
        {
            try
            {
                return driver.FindElement(locator).Displayed;
            }
            catch (NoSuchElementException)
            {
                string locatorValue = locator.ToString().Split(':')[1].Trim();
                Console.WriteLine($"{locatorValue} field not found.");
                return false;
            }
        }

        public bool IsAllDisplayed()
        {
            return IsDisplayed(FirstNameInput) &&
                    IsDisplayed(LastNameInput) && 
                    IsDisplayed(DateOfBirthInput) &&
                    IsDisplayed(GenderInput) &&
                    IsDisplayed(StreetnameInput) &&
                    IsDisplayed(HousenumberInput) &&
                    IsDisplayed(PostalcodeInput) &&
                    IsDisplayed(CitizenServiceNumberInput) &&
                    IsDisplayed(PhoneNumberInput) &&
                    IsDisplayed(EmailInput) &&
                    IsDisplayed(PasswordInput) &&
                    IsDisplayed(RegisterButton);
        }

        public bool IsErrorMessageDisplayed()
        {
            try
            {
                return driver.FindElement(ErrorMessage).Displayed;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Error message not found.");
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
                Console.WriteLine("Error message element not found.");
                return string.Empty;
            }
        }
    }
}
