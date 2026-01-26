using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US3._7
{
    [TestClass]
    public class _3_7a_2 : BaseTest
    {
        [TestMethod]
        public void TC_3_7a_2_SaveWithoutChanges_SaveButtonDisabled()
        {
            // Stap 1: Login als Admin
            LogStep(1, "Logging in as administrator...");
            adminLoginPage.Navigate();
            wait.Until(d => adminLoginPage.IsPasswordInputDisplayed());
            adminLoginPage.PerformLogin("admin@example.com", "Admin123");
            wait.Until(d => !adminLoginPage.IsPasswordInputDisplayed());
            LogSuccess(1, "Admin login successful.");

            // Stap 2: Navigatie naar Gebruikersbeheer
            LogStep(2, "Navigating to User Management page...");
            userManagementPage.Navigate();
            wait.Until(d => d.FindElements(By.ClassName("user-row")).Count > 0);
            LogSuccess(2, "User Management page loaded with users.");

            // Stap 3: Identificatie eerste gebruiker
            LogStep(3, "Finding the first user row in the list...");
            var userRows = driver.FindElements(By.ClassName("user-row"));
            Assert.IsTrue(userRows.Count > 0, "Geen gebruikers gevonden op de pagina.");

            var firstUserRow = userRows[0];
            LogSuccess(3, "First user row identified.");

            // Stap 4: Controleer status van de Save button zonder wijzigingen
            LogStep(4, "Verifying that the 'Save' button is disabled when no changes are made...");
            var saveButton = firstUserRow.FindElement(By.ClassName("save-btn"));

            // Verifieer dat de knop disabled is (via de 'disabled' attribute of Selenium Enabled property)
            bool isBtnDisabled = !saveButton.Enabled || saveButton.GetAttribute("disabled") == "true";

            Assert.IsTrue(isBtnDisabled, "De Save button zou disabled moeten zijn als er geen wijzigingen zijn aangebracht.");
            LogInfo("Save button status: Disabled (Correct)");
            LogSuccess(4, "Verified: Save button is properly disabled without changes.");

            // Stap 5: Optionele verificatie - Wordt de knop enabled na een wijziging?
            LogStep(5, "Verifying button becomes enabled after changing a role...");
            var roleSelect = firstUserRow.FindElement(By.ClassName("role-select"));
            var selectElement = new OpenQA.Selenium.Support.UI.SelectElement(roleSelect);

            // Selecteer een andere optie dan de huidige
            string currentRole = selectElement.SelectedOption.Text;
            var otherOption = selectElement.Options.FirstOrDefault(o => o.Text != currentRole);

            if (otherOption != null)
            {
                selectElement.SelectByText(otherOption.Text);
                LogInfo($"Changed role to: {otherOption.Text}");

                Assert.IsTrue(saveButton.Enabled, "De Save button zou enabled moeten worden na een wijziging.");
                LogSuccess(5, "Verified: Save button enabled after user input.");
            }
            else
            {
                LogInfo("Skipping Step 5: No alternative roles available to test state change.");
            }

            LogSuccess(6, "Test completed successfully: Save button state logic is correct.");
        }
    }
}