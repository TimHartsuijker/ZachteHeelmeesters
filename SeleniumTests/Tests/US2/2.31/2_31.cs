using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _2_31
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private RegistrationPage registrationPage;

        [TestInitialize]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            registrationPage = new RegistrationPage(driver);
        }

        [TestCleanup]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void InputFieldsAreAvailable()
        {
            Console.WriteLine("Test started: Input fields are available");

            registrationPage.Navigate();
            Assert.IsTrue(registrationPage.IsAllDisplayed(),
                "One or more registration input fields or the register button are not displayed.");
        }

        [TestMethod]
        public void RegistrationWithEmptyFieldsFail()
        {
            string FIRSTNAME = "New";
            string LASTNAME = "User";
            string STREETNAME = "Main Street";
            string HOUSENUMBER = "123";
            string POSTALCODE = "1234AB";
            string CITIZENSERVICENUMBER = "123456789";
            string DATEOFBIRTH = "01-01-1990";
            string GENDER = "Man";
            string PHONE_NUMBER = "0611223344";
            string EMAIL = "newuser@example.com";
            string PASSWORD = "NewUserPassword";

            Console.WriteLine("Test started: Registration with empty fields");

            // Empty first name
            registrationPage.Register(
                "", LASTNAME, STREETNAME, HOUSENUMBER, POSTALCODE,
                CITIZENSERVICENUMBER, DATEOFBIRTH, GENDER, PHONE_NUMBER, EMAIL, PASSWORD
            );
            wait.Until(d => registrationPage.IsErrorMessageDisplayed());
            Assert.AreEqual("All fields must be filled in", registrationPage.GetErrorMessage());

            // Empty last name
            registrationPage.Register(
                FIRSTNAME, "", STREETNAME, HOUSENUMBER, POSTALCODE,
                CITIZENSERVICENUMBER, DATEOFBIRTH, GENDER, PHONE_NUMBER, EMAIL, PASSWORD
            );
            wait.Until(d => registrationPage.IsErrorMessageDisplayed());
            Assert.AreEqual("All fields must be filled in", registrationPage.GetErrorMessage());

            // Empty phone number
            registrationPage.Register(
                FIRSTNAME, LASTNAME, STREETNAME, HOUSENUMBER, POSTALCODE,
                CITIZENSERVICENUMBER, DATEOFBIRTH, GENDER, "", EMAIL, PASSWORD
            );
            wait.Until(d => registrationPage.IsErrorMessageDisplayed());
            Assert.AreEqual("All fields must be filled in", registrationPage.GetErrorMessage());

            // Empty email
            registrationPage.Register(
                FIRSTNAME, LASTNAME, STREETNAME, HOUSENUMBER, POSTALCODE,
                CITIZENSERVICENUMBER, DATEOFBIRTH, GENDER, PHONE_NUMBER, "", PASSWORD
            );
            wait.Until(d => registrationPage.IsErrorMessageDisplayed());
            Assert.AreEqual("All fields must be filled in", registrationPage.GetErrorMessage());

            // Empty password
            registrationPage.Register(
                FIRSTNAME, LASTNAME, STREETNAME, HOUSENUMBER, POSTALCODE,
                CITIZENSERVICENUMBER, DATEOFBIRTH, GENDER, PHONE_NUMBER, EMAIL, ""
            );
            wait.Until(d => registrationPage.IsErrorMessageDisplayed());
            Assert.AreEqual("All fields must be filled in", registrationPage.GetErrorMessage());
        }

        [TestMethod]
        public void SuccesfulRegistration()
        {
            string FIRSTNAME = "New";
            string LASTNAME = "Patient1";
            string STREETNAME = "Main Street";
            string HOUSENUMBER = "123";
            string POSTALCODE = "1234AB";
            string CITIZENSERVICENUMBER = "123456789";
            string DATEOFBIRTH = "01-01-1990";
            string GENDER = "Man";
            string PHONE_NUMBER = "0611223344";
            string EMAIL = "newpatient1@example.com";
            string PASSWORD = "NewPatient1Password";

            Console.WriteLine("Test started: Succesful Registration");

            registrationPage.Register(
                FIRSTNAME, LASTNAME, STREETNAME, HOUSENUMBER, POSTALCODE,
                CITIZENSERVICENUMBER, DATEOFBIRTH, GENDER, PHONE_NUMBER, EMAIL, PASSWORD
            );

            Console.WriteLine("Waiting until the newly registered user is redirected to the login page");
            wait.Until(d => d.Url.Contains("/login"));
            Assert.IsTrue(driver.Url.Contains("/login"),
                "Newly registered user has not been redirected to the login page.");
        }

        [TestMethod]
        public void RegistrationWithExistingEmailFails()
        {
            string FIRSTNAME = "New";
            string LASTNAME = "User";
            string STREETNAME = "Main Street";
            string HOUSENUMBER = "123";
            string POSTALCODE = "1234AB";
            string CITIZENSERVICENUMBER = "123456789";
            string DATEOFBIRTH = "01-01-1990";
            string GENDER = "Man";
            string PHONE_NUMBER = "0611223344";
            string EXISTING_EMAIL = "gebruiker@example.com"; // Existing email
            string PASSWORD = "NewUserPassword";

            Console.WriteLine("Test started: Registration with existing email fails");

            registrationPage.Register(
                FIRSTNAME, LASTNAME, STREETNAME, HOUSENUMBER, POSTALCODE,
                CITIZENSERVICENUMBER, DATEOFBIRTH, GENDER, PHONE_NUMBER, EXISTING_EMAIL, PASSWORD
            );

            wait.Until(d => registrationPage.IsErrorMessageDisplayed());
            Assert.AreEqual("The email that was used is already registered", registrationPage.GetErrorMessage());
        }

        [TestMethod]
        public void RegistrationsWithInvalidEmailsFail()
        {
            string FIRSTNAME = "New";
            string LASTNAME = "User";
            string STREETNAME = "Main Street";
            string HOUSENUMBER = "123";
            string POSTALCODE = "1234AB";
            string CITIZENSERVICENUMBER = "123456789";
            string DATEOFBIRTH = "01-01-1990";
            string GENDER = "Man";
            string PHONE_NUMBER = "0611223344";
            string PASSWORD = "NewUserPassword";

            string[] INVALID_EMAILS = new string[]
            {
                "gebruikerexample.com",
                "gebruiker@examplecom",
                "@example.com",
                "gebruiker@.com",
                "gebruiker@example.",
                "@example."
            };

            Console.WriteLine("Test started: Registrations with invalid emails fail");

            foreach (var invalidEmail in INVALID_EMAILS)
            {
                Console.WriteLine($"Testcase: {invalidEmail}");
                registrationPage.Register(
                    FIRSTNAME, LASTNAME, STREETNAME, HOUSENUMBER, POSTALCODE,
                    CITIZENSERVICENUMBER, DATEOFBIRTH, GENDER, PHONE_NUMBER, invalidEmail, PASSWORD
                );

                wait.Until(d => registrationPage.IsErrorMessageDisplayed());
                Assert.AreEqual("The email that was used is invalid", registrationPage.GetErrorMessage());
            }
        }
    }
}
