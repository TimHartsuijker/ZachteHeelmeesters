using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace US2._31
{
    [TestClass]
    public class _2_31 : BaseTest
    {
        // Standaard data voor een valide registratie
        private readonly Dictionary<string, string> _validData = new Dictionary<string, string>
        {
            { "FirstName", "New" },
            { "LastName", "User" },
            { "Street", "Main Street" },
            { "HouseNumber", "123" },
            { "PostalCode", "1234AB" },
            { "BSN", "000011009" },
            { "DOB", "01-01-1990" },
            { "Gender", "Man" },
            { "Phone", "0611223344" },
            { "Email", "newuser@example.com" },
            { "Password", "NewUserPassword" }
        };

        [TestMethod]
        public void TC_2_31_01_InputFieldsAreAvailable()
        {
            LogStep(1, "Navigating to registration page...");
            registrationPage.Navigate();
            LogSuccess(1, "Registration page loaded.");

            LogStep(2, "Verifying visibility of all registration input fields...");
            Assert.IsTrue(registrationPage.IsAllDisplayed(),
                "One or more registration input fields or the register button are not displayed.");
            LogSuccess(2, "All required input fields and buttons are visible.");
        }

        [TestMethod]
        public void TC_2_31_02_RegistrationWithEmptyFieldsFail()
        {
            const string ERROR_TEXT = "Alle velden moeten ingevuld zijn.";
            LogStep(1, "Starting systematic validation check for all mandatory fields...");

            // We lopen door elk veld in onze validData dictionary
            foreach (var fieldToEmpty in _validData.Keys)
            {
                registrationPage.Navigate();
                LogStep(2, $"Testing registration with empty field: {fieldToEmpty}");

                // Maak een kopie van de valide data en zet het huidige veld op leeg
                var testData = new Dictionary<string, string>(_validData);
                testData[fieldToEmpty] = "";

                registrationPage.Register(
                    testData["FirstName"], testData["LastName"], testData["Street"],
                    testData["HouseNumber"], testData["PostalCode"], testData["BSN"],
                    testData["DOB"], testData["Gender"], testData["Phone"],
                    5, testData["Email"], testData["Password"]
                );

                wait.Until(d => registrationPage.IsErrorMessageDisplayed());
                string actualError = registrationPage.GetErrorMessageText();

                Assert.AreEqual(ERROR_TEXT, actualError, $"Validation failed for empty field: {fieldToEmpty}");
                LogSuccess(2, $"System correctly blocked registration when {fieldToEmpty} was empty.");
            }
            LogSuccess(1, "All mandatory field validations passed successfully.");
        }

        [TestMethod]
        public void TC_2_31_03_SuccesfulRegistration()
        {
            string email = "newpatient" + DateTime.Now.Ticks + "@example.com"; // Uniek email per run

            LogStep(1, "Navigating to registration page...");
            registrationPage.Navigate();

            LogStep(2, $"Performing full registration for: {email}");
            registrationPage.Register(
                _validData["FirstName"], "PatientUnique", _validData["Street"],
                _validData["HouseNumber"], _validData["PostalCode"], "010101001",
                _validData["DOB"], _validData["Gender"], _validData["Phone"],
                5, email, _validData["Password"]
            );
            LogSuccess(2, "Registration data submitted.");

            LogStep(3, "Waiting for automatic redirect to login page...");
            wait.Until(d => d.Url.Contains("/login"));
            Assert.IsTrue(driver.Url.Contains("/login"), "Newly registered user was not redirected to the login page.");
            LogSuccess(3, "Registration successful and redirected to login portal.");
        }

        [TestMethod]
        public void TC_2_31_04_RegistrationWithExistingEmailFails()
        {
            const string EXISTING_EMAIL = "gebruiker@example.com";

            LogStep(1, "Navigating to registration page...");
            registrationPage.Navigate();

            LogStep(2, $"Attempting registration with already registered email: {EXISTING_EMAIL}");
            registrationPage.Register(
                _validData["FirstName"], _validData["LastName"], _validData["Street"],
                _validData["HouseNumber"], _validData["PostalCode"], "011101001",
                _validData["DOB"], _validData["Gender"], _validData["Phone"],
                5, EXISTING_EMAIL, _validData["Password"]
            );

            LogStep(3, "Verifying duplicate email error message...");
            wait.Until(d => registrationPage.IsErrorMessageDisplayed());
            Assert.AreEqual("Dit email is al geregistreerd.", registrationPage.GetErrorMessageText());
            LogSuccess(3, "Registration correctly blocked for existing email.");
        }

        [TestMethod]
        public void TC_2_31_05_RegistrationsWithInvalidEmailsFail()
        {
            string[] INVALID_EMAILS = { "gebruikerexample.com", "gebruiker@", "@example.com", "gebruiker@.com" };

            LogStep(1, "Starting validation tests for invalid email formats...");
            registrationPage.Navigate();

            foreach (var invalidEmail in INVALID_EMAILS)
            {
                LogStep(2, $"Testing invalid email: {invalidEmail}");
                registrationPage.Register(
                    _validData["FirstName"], _validData["LastName"], _validData["Street"],
                    _validData["HouseNumber"], _validData["PostalCode"], "010101011",
                    _validData["DOB"], _validData["Gender"], _validData["Phone"],
                    5, invalidEmail, _validData["Password"]
                );

                // We verwachten dat we op de registratiepagina blijven of een error zien (geen redirect naar login)
                bool redirected = driver.Url.Contains("/login");
                Assert.IsFalse(redirected, $"Registratie met incorrecte email '{invalidEmail}' is onterecht geaccepteerd!");
                LogSuccess(2, $"Format '{invalidEmail}' was correctly rejected.");
            }
            LogSuccess(1, "All invalid email formats were correctly blocked.");
        }
    }
}