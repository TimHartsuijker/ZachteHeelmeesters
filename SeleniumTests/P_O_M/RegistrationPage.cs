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

        public void EnterFirstName(string firstname)
        {
            Console.WriteLine($"Entering FirstName: {firstname}");

            var input = driver.FindElement(FirstNameInput);
            input.Clear();
            input.SendKeys(firstname);
        }

        public void EnterLastName(string lastname)
        {
            Console.WriteLine($"Entering LastName: {lastname}");

            var input = driver.FindElement(LastNameInput);
            input.Clear();
            input.SendKeys(lastname);
        }

        public void EnterPhoneNumber(string phone_number)
        {
            Console.WriteLine($"Entering PhoneNumber: {phone_number}");

            var input = driver.FindElement(PhoneNumberInput);
            input.Clear();
            input.SendKeys(phone_number);
        }

        public void EnterEmail(string email)
        {
            Console.WriteLine($"Entering Email: {email}");

            var input = driver.FindElement(EmailInput);
            input.Clear();
            input.SendKeys(email);
        }

        public void EnterPassword(string password)
        {
            Console.WriteLine($"Entering Password: {password}");

            var input = driver.FindElement(PasswordInput);
            input.Clear();
            input.SendKeys(password);
        }

        public void Register(string firstname, string lastname, string phone_number, string email, string password)
        {
            Navigate();

            if (!IsAllDisplayed())
            {
                throw new NoSuchElementException("One or more registration fields are not displayed.");
            }
            
            Console.WriteLine("Registrationpage loaded.");

            
            EnterFirstName(firstname);
            EnterLastName(lastname);
            EnterPhoneNumber(phone_number);
            EnterEmail(email);
            EnterPassword(password);

            Console.WriteLine("Clicking the register button");
            driver.FindElement(RegisterButton).Click();
        }

        public bool IsFirstNameFieldDisplayed()
        {
            try
            {
                return driver.FindElement(FirstNameInput).Displayed;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("First name field not found.");
                return false;
            }
        }

        public bool IsLastNameFieldDisplayed()
        {
            try 
            {
                return driver.FindElement(LastNameInput).Displayed;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Last name field not found.");
                return false;
            }
        }

        public bool IsPhoneNumberFieldDisplayed()
        {
            try 
            {
                return driver.FindElement(PhoneNumberInput).Displayed;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Phone number field not found.");
                return false;
            }
        }

        public bool IsEmailFieldDisplayed()
        {
            try 
            {
                return driver.FindElement(EmailInput).Displayed;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Email field not found.");
                return false;
            }
        }

        public bool IsPasswordFieldDisplayed()
        {
            try
            {
                return driver.FindElement(PasswordInput).Displayed;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Password field not found.");
                return false;
            }
        }

        public bool IsRegisterButtonDisplayed()
        {
            try
            {
                return driver.FindElement(RegisterButton).Displayed;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Register button not found.");
                return false;
            }
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

        public bool IsAllDisplayed()
        {
                return IsFirstNameFieldDisplayed() &&
            IsLastNameFieldDisplayed() &&
            IsPhoneNumberFieldDisplayed() &&
            IsEmailFieldDisplayed() &&
            IsPasswordFieldDisplayed() &&
            IsRegisterButtonDisplayed();
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
