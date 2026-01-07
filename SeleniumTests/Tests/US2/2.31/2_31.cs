using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;
using System;

namespace SeleniumTests;

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
        string PHONE_NUMBER = "0611223344";
        string EMAIL = "newuser@example.com";
        string PASSWORD = "NewUserPassword";

        Console.WriteLine("Test started: Registration with empty fields");
        // Test with empty first name
        registrationPage.Register("", LASTNAME, PHONE_NUMBER, EMAIL, PASSWORD);
        wait.Until(d => registrationPage.IsErrorMessageDisplayed());
        Assert.AreEqual("All fields must be filled in", registrationPage.GetErrorMessage());

        // Test with empty last name
        registrationPage.Register(FIRSTNAME, "", PHONE_NUMBER, EMAIL, PASSWORD);
        wait.Until(d => registrationPage.IsErrorMessageDisplayed());
        Assert.AreEqual("All fields must be filled in", registrationPage.GetErrorMessage());

        // Test with empty phone number
        registrationPage.Register(FIRSTNAME, LASTNAME, "", EMAIL, PASSWORD);
        wait.Until(d => registrationPage.IsErrorMessageDisplayed());
        Assert.AreEqual("All fields must be filled in", registrationPage.GetErrorMessage());

        // Test with empty email
        registrationPage.Register(FIRSTNAME, LASTNAME, PHONE_NUMBER, "", PASSWORD);
        wait.Until(d => registrationPage.IsErrorMessageDisplayed());
        Assert.AreEqual("All fields must be filled in", registrationPage.GetErrorMessage());

        // Test with empty password
        registrationPage.Register(FIRSTNAME, LASTNAME, PHONE_NUMBER, EMAIL, "");
        wait.Until(d => registrationPage.IsErrorMessageDisplayed());
        Assert.AreEqual("All fields must be filled in", registrationPage.GetErrorMessage());
    }

    [TestMethod]
    public void SuccesfulRegistration()
    {
        string FIRSTNAME = "New";
        string LASTNAME = "Patient1";
        string PHONE_NUMBER = "0611223344";
        string EMAIL = "newpatient1@example.com";
        string PASSWORD = "NewPatient1Password";

        Console.WriteLine("Test started: Succesful Registration");

        registrationPage.Register(FIRSTNAME, LASTNAME, PHONE_NUMBER, EMAIL, PASSWORD);

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
        string PHONE_NUMBER = "0611223344";
        string EXISTING_EMAIL = "gebruiker@example.com"; // Existing email
        string PASSWORD = "NewUserPassword";

        Console.WriteLine("Test started: Registration with existing email fails");
        registrationPage.Register(FIRSTNAME, LASTNAME, PHONE_NUMBER, EXISTING_EMAIL, PASSWORD);
        Console.WriteLine("Waiting until the error message is displayed");

        wait.Until(d => registrationPage.IsErrorMessageDisplayed());

        Assert.AreEqual("The email that was used is already registered", registrationPage.GetErrorMessage());
    }

    [TestMethod]
    public void RegistrationsWithInvalidEmailsFail()
    {
        string FIRSTNAME = "New";
        string LASTNAME = "User";
        string PHONE_NUMBER = "0611223344";
        string PASSWORD = "NewUserPassword";

        // Invalid email formats
        string INVALID_EMAIL1 = "gebruikerexample.com";
        string INVALID_EMAIL2 = "gebruiker@examplecom";
        string INVALID_EMAIL3 = "@example.com";
        string INVALID_EMAIL4 = "gebruiker@.com";
        string INVALID_EMAIL5 = "gebruiker@example.";
        string INVALID_EMAIL6 = "@example.";


        Console.WriteLine("Test started: Registrations with invalid emails fail");


        Console.WriteLine($"Testcase 1: {INVALID_EMAIL1}");
        registrationPage.Register(FIRSTNAME, LASTNAME, PHONE_NUMBER, INVALID_EMAIL1, PASSWORD);
        Console.WriteLine("Waiting until the error message is displayed");
        wait.Until(d => registrationPage.IsErrorMessageDisplayed());
        Assert.AreEqual("The email that was used is invalid", registrationPage.GetErrorMessage());


        Console.WriteLine($"Testcase 2: {INVALID_EMAIL2}");
        registrationPage.Register(FIRSTNAME, LASTNAME, PHONE_NUMBER, INVALID_EMAIL2, PASSWORD);
        Console.WriteLine("Waiting until the error message is displayed");
        wait.Until(d => registrationPage.IsErrorMessageDisplayed());
        Assert.AreEqual("The email that was used is invalid", registrationPage.GetErrorMessage());


        Console.WriteLine($"Testcase 3: {INVALID_EMAIL3}");
        registrationPage.Register(FIRSTNAME, LASTNAME, PHONE_NUMBER, INVALID_EMAIL3, PASSWORD);
        Console.WriteLine("Waiting until the error message is displayed");
        wait.Until(d => registrationPage.IsErrorMessageDisplayed());
        Assert.AreEqual("The email that was used is invalid", registrationPage.GetErrorMessage());


        Console.WriteLine($"Testcase 4: {INVALID_EMAIL4}");
        registrationPage.Register(FIRSTNAME, LASTNAME, PHONE_NUMBER, INVALID_EMAIL4, PASSWORD);
        Console.WriteLine("Waiting until the error message is displayed");
        wait.Until(d => registrationPage.IsErrorMessageDisplayed());
        Assert.AreEqual("The email that was used is invalid", registrationPage.GetErrorMessage());


        Console.WriteLine($"Testcase 5: {INVALID_EMAIL5}");
        registrationPage.Register(FIRSTNAME, LASTNAME, PHONE_NUMBER, INVALID_EMAIL5, PASSWORD);
        Console.WriteLine("Waiting until the error message is displayed");
        wait.Until(d => registrationPage.IsErrorMessageDisplayed());
        Assert.AreEqual("The email that was used is invalid", registrationPage.GetErrorMessage());


        Console.WriteLine($"Testcase 6: {INVALID_EMAIL6}");
        registrationPage.Register(FIRSTNAME, LASTNAME, PHONE_NUMBER, INVALID_EMAIL6, PASSWORD);
        Console.WriteLine("Waiting until the error message is displayed");
        wait.Until(d => registrationPage.IsErrorMessageDisplayed());
        Assert.AreEqual("The email that was used is invalid", registrationPage.GetErrorMessage());
    }
}