using OpenQA.Selenium;
using System;

namespace SeleniumTests.Pages
{
    public class RegistrationPage (IWebDriver driver) : BasePage(driver)
    { 
        public static string Path => "/register";

        // Locators
        private static By FirstNameInput => By.Id("firstname");
        private static By LastNameInput => By.Id("lastname");
        private static By DateOfBirthInput => By.Id("dob");
        private static By GenderSelect => By.Id("gender");
        private static By StreetnameInput => By.Id("streetname");
        private static By HousenumberInput => By.Id("housenumber");
        private static By PostalcodeInput => By.Id("postalcode");
        private static By CitizenServiceNumberInput => By.Id("csn");
        private static By PhoneNumberInput => By.Id("phonenumber");
        private static By GpSelect => By.Id("gp-id");
        private static By EmailInput => By.Id("email");
        private static By PasswordInput => By.Id("password");
        private static By RegisterButton => By.Id("register-btn");
        private static By ErrorMessage => By.Id("error-message");

        public void Navigate()
        {
            NavigateTo(BaseUrl + Path);
        }

        public void Register(string firstname, string lastname, string streetname, string housenumber,
                             string postalcode, string csn, string dob, string gender,
                             string phone, int gp_id, string email, string password)
        {
            Navigate();

            // Gebruik de nieuwe IsAllDisplayed die gebruik maakt van BasePage
            if (!IsAllDisplayed())
            {
                throw new NoSuchElementException("One or more registration fields are not displayed.");
            }

            Console.WriteLine("Registration page loaded. Entering data...");

            // Gebruik de SendKeys helper uit de BasePage
            SendKeys(FirstNameInput, firstname);
            SendKeys(LastNameInput, lastname);
            SendKeys(EmailInput, email);
            SendKeys(PasswordInput, password);
            SendKeys(StreetnameInput, streetname);
            SendKeys(HousenumberInput, housenumber);
            SendKeys(PostalcodeInput, postalcode);
            SendKeys(CitizenServiceNumberInput, csn);
            SendKeys(DateOfBirthInput, dob);
            SelectByValue(GenderSelect, gender);
            SendKeys(PhoneNumberInput, phone);
            SelectByValue(GpSelect, gp_id.ToString());

            Console.WriteLine("Clicking the register button");
            Click(RegisterButton);
        }

        public bool IsAllDisplayed()
        {
            Wait.Until(driver => IsElementDisplayed(RegisterButton));
            return IsElementDisplayed(FirstNameInput) &&
                    IsElementDisplayed(LastNameInput) &&
                    IsElementDisplayed(DateOfBirthInput) &&
                    IsElementDisplayed(GenderSelect) &&
                    IsElementDisplayed(StreetnameInput) &&
                    IsElementDisplayed(HousenumberInput) &&
                    IsElementDisplayed(PostalcodeInput) &&
                    IsElementDisplayed(CitizenServiceNumberInput) &&
                    IsElementDisplayed(PhoneNumberInput) &&
                    IsElementDisplayed(EmailInput) &&
                    IsElementDisplayed(PasswordInput) &&
                    IsElementDisplayed(RegisterButton);
        }

        public bool IsErrorMessageDisplayed() => IsElementDisplayed(ErrorMessage);

        public string GetErrorMessageText() => GetText(ErrorMessage);
    }
}
