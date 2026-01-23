using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US1._19
{
    [TestClass]
    public class _1_19_2 : BaseTest
    {
        [TestMethod]
        public void TC_1_19_2_PasswordInputIsPresentAndAcceptsInput()
        {
            const string TEST_PASSWORD = "Test123!";

            // Stap 1: Navigatie
            LogStep(1, "Navigating to login page...");
            loginPage.Navigate();
            LogSuccess(1, "Login page loaded successfully.");

            // Stap 2: Zichtbaarheid controleren
            LogStep(2, "Verifying if password input field is visible...");
            wait.Until(d => loginPage.IsPasswordInputDisplayed());
            Assert.IsTrue(loginPage.IsPasswordInputDisplayed(), "Het wachtwoordveld is niet zichtbaar.");
            LogSuccess(2, "Password input field is visible.");

            // Stap 3: Focus/Klikken
            LogStep(3, "Selecting password input field...");
            loginPage.ClickPasswordInput();

            Assert.IsTrue(loginPage.IsPasswordInputFocused(), "Het wachtwoordveld kan geen focus krijgen.");
            LogSuccess(3, "Password input field clicked and focused.");

            // Stap 4: Tekst invoer
            LogStep(4, $"Entering password: {TEST_PASSWORD}");
            loginPage.EnterPassword(TEST_PASSWORD);

            Assert.AreEqual(TEST_PASSWORD, loginPage.GetPasswordInputValue(), "Het wachtwoordveld accepteert geen tekst.");
            LogSuccess(4, "Password correctly entered and verified.");

            // Stap 5: Finale controle
            LogStep(5, "Verifying input field state...");
            LogInfo("Input field value matches the test password.");
            LogSuccess(5, "Password input verification complete.");
        }
    }
}