using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US1._19
{
    [TestClass]
    public class _1_19_5 : BaseTest
    {
        // ------------------------
        // 1. Goed e-mail + fout wachtwoord
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_CorrectEmail_WrongPassword()
        {
            const string CORRECT_EMAIL = "gebruiker@example.com";
            const string WRONG_PASSWORD = "FoutWw123";

            LogStep(1, "Navigating to login page...");
            loginPage.Navigate();
            wait.Until(d => loginPage.IsPasswordInputDisplayed());
            LogSuccess(1, "Login page loaded successfully.");

            LogStep(2, $"Attempting login with correct email and wrong password...");
            loginPage.PerformLogin(CORRECT_EMAIL, WRONG_PASSWORD);
            LogSuccess(2, "Login attempt submitted.");

            LogStep(3, "Verifying error message for incorrect credentials...");
            wait.Until(d => loginPage.IsErrorDisplayed());
            var errorText = loginPage.GetErrorMessageText();
            LogInfo($"Error message found: {errorText}");

            Assert.AreEqual("Inloggegevens zijn incorrect", errorText);
            LogSuccess(3, "Correct error message displayed for wrong password.");
        }

        // ------------------------
        // 2. Fout e-mail + goed wachtwoord
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_WrongEmail_CorrectPassword()
        {
            const string WRONG_EMAIL = "gebruikerexample.com";
            const string CORRECT_PASSWORD = "Wachtwoord123";

            LogStep(1, "Navigating to login page...");
            loginPage.Navigate();
            wait.Until(d => loginPage.IsPasswordInputDisplayed());
            LogSuccess(1, "Login page loaded successfully.");

            LogStep(2, $"Attempting login with wrong email format and correct password...");
            loginPage.PerformLogin(WRONG_EMAIL, CORRECT_PASSWORD);
            LogSuccess(2, "Login attempt submitted.");

            LogStep(3, "Verifying error message for incorrect credentials...");
            wait.Until(d => loginPage.IsErrorDisplayed());
            var errorText = loginPage.GetErrorMessageText();
            LogInfo($"Error message found: {errorText}");

            Assert.AreEqual("Inloggegevens zijn incorrect", errorText);
            LogSuccess(3, "Correct error message displayed for wrong email.");
        }

        // ------------------------
        // 3. Beide verkeerd
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_WrongEmail_WrongPassword()
        {
            const string WRONG_EMAIL = "gebruikerexample.com";
            const string WRONG_PASSWORD = "Wachtwoord";

            LogStep(1, "Navigating to login page...");
            loginPage.Navigate();
            wait.Until(d => loginPage.IsPasswordInputDisplayed());
            LogSuccess(1, "Login page loaded successfully.");

            LogStep(2, "Attempting login with both email and password incorrect...");
            loginPage.PerformLogin(WRONG_EMAIL, WRONG_PASSWORD);
            LogSuccess(2, "Login attempt submitted.");

            LogStep(3, "Verifying error message for incorrect credentials...");
            wait.Until(d => loginPage.IsErrorDisplayed());
            var errorText = loginPage.GetErrorMessageText();
            LogInfo($"Error message found: {errorText}");

            Assert.AreEqual("Inloggegevens zijn incorrect", errorText);
            LogSuccess(3, "Correct error message displayed for both fields incorrect.");
        }

        // ------------------------
        // 4. E-mail leeg + wachtwoord ingevuld
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_EmptyEmail_CorrectPassword()
        {
            const string CORRECT_PASSWORD = "Wachtwoord123";

            LogStep(1, "Navigating to login page...");
            loginPage.Navigate();
            wait.Until(d => loginPage.IsPasswordInputDisplayed());
            LogSuccess(1, "Login page loaded successfully.");

            LogStep(2, "Attempting login with empty email field...");
            loginPage.PerformLogin("", CORRECT_PASSWORD);
            LogSuccess(2, "Login attempt submitted.");

            LogStep(3, "Verifying validation message for empty fields...");
            wait.Until(d => loginPage.IsEmptyDisplayed());
            var errorText = loginPage.GetEmptyMessageText();
            LogInfo($"Validation message found: {errorText}");

            Assert.AreEqual("Gegevens moeten ingevuld zijn", errorText);
            LogSuccess(3, "Correct validation message displayed for empty email.");
        }

        // ------------------------
        // 5. E-mail ingevuld + wachtwoord leeg
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_CorrectEmail_EmptyPassword()
        {
            const string CORRECT_EMAIL = "gebruiker@example.com";

            LogStep(1, "Navigating to login page...");
            loginPage.Navigate();
            wait.Until(d => loginPage.IsPasswordInputDisplayed());
            LogSuccess(1, "Login page loaded successfully.");

            LogStep(2, "Attempting login with empty password field...");
            loginPage.PerformLogin(CORRECT_EMAIL, "");
            LogSuccess(2, "Login attempt submitted.");

            LogStep(3, "Verifying validation message for empty fields...");
            wait.Until(d => loginPage.IsEmptyDisplayed());
            var errorText = loginPage.GetEmptyMessageText();
            LogInfo($"Validation message found: {errorText}");

            Assert.AreEqual("Gegevens moeten ingevuld zijn", errorText);
            LogSuccess(3, "Correct validation message displayed for empty password.");
        }

        // ------------------------
        // 6. Beide leeg
        // ------------------------
        [TestMethod]
        public void TC_1_19_5_BothFieldsEmpty()
        {
            LogStep(1, "Navigating to login page...");
            loginPage.Navigate();
            wait.Until(d => loginPage.IsPasswordInputDisplayed());
            LogSuccess(1, "Login page loaded successfully.");

            LogStep(2, "Attempting login with both fields empty...");
            loginPage.PerformLogin("", "");
            LogSuccess(2, "Login attempt submitted.");

            LogStep(3, "Verifying validation message for empty fields...");
            wait.Until(d => loginPage.IsEmptyDisplayed());
            var errorText = loginPage.GetEmptyMessageText();
            LogInfo($"Validation message found: {errorText}");

            Assert.AreEqual("Gegevens moeten ingevuld zijn", errorText);
            LogSuccess(3, "Correct validation message displayed for both fields empty.");
        }
    }
}