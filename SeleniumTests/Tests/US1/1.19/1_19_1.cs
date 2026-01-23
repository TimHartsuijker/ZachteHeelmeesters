using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US1._19
{
    [TestClass]
    public class _1_19_1 : BaseTest
    {
        [TestMethod]
        public void TC_1_19_1_EmailInputIsPresentAndAcceptsInput()
        {
            const string TEST_EMAIL = "test@example.com";

            // Stap 1: Navigatie
            LogStep(1, "Navigating to login page...");
            loginPage.Navigate();
            LogSuccess(1, "Login page loaded successfully.");

            // Stap 2: Zichtbaarheid controleren
            LogStep(2, "Verifying if email input field is visible...");
            wait.Until(d => loginPage.IsEmailInputDisplayed());
            Assert.IsTrue(loginPage.IsEmailInputDisplayed(), "Het e-mailadres veld is niet zichtbaar.");
            LogSuccess(2, "Email input field is visible.");

            // Stap 3: Focus/Klikken
            LogStep(3, "Selecting email input field...");
            loginPage.ClickEmailInput();

            Assert.IsTrue(loginPage.IsEmailInputFocused(), "Het e-mailadres veld kan geen focus krijgen.");
            LogSuccess(3, "Email input field clicked and focused.");

            // Stap 4: Tekst invoer
            LogStep(4, $"Entering email address: {TEST_EMAIL}");
            loginPage.EnterEmail(TEST_EMAIL);

            Assert.AreEqual(TEST_EMAIL, loginPage.GetEmailInputValue(), "Het e-mailadres veld accepteert geen tekst.");
            LogSuccess(4, "Email correctly entered and verified.");

            // Optionele extra info
            LogStep(5, "Final verification of input state...");
            LogInfo($"Verified value: {loginPage.GetEmailInputValue()}");
            LogSuccess(5, "Input verification complete.");
        }
    }
}