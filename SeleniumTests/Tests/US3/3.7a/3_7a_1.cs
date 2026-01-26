using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Base;

namespace US3._7
{
    [TestClass]
    public class _3_7a_1 : BaseTest
    {
        public _3_7a_1()
        {
            useDbContext = true;
        }

        [TestMethod]
        public void TC_3_7a_1_RoleChange_UserRoleIsChanged()
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
            var userRows = userManagementPage.GetUserRows();
            Assert.IsTrue(userRows.Count > 0, "Geen gebruikers gevonden om een rolwijziging op te testen.");
            LogSuccess(2, "User Management page loaded with data.");

            // Stap 3: Identificatie eerste gebruiker
            LogStep(3, "Identifying the first user in the list...");
            var firstUserRow = userRows[0];
            var roleSelect = userManagementPage.GetSelectByRow(firstUserRow);
            var selectElement = new SelectElement(roleSelect);
            LogSuccess(3, "First user row and role selector found.");

            // Stap 4: Rol bepalen en wijzigen
            LogStep(4, "Determining current role and selecting a new one...");
            string currentRole = selectElement.SelectedOption.Text;
            var availableRoles = selectElement.Options.Select(o => o.Text).ToList();

            string newRole = availableRoles.FirstOrDefault(r => r != currentRole)!;
            Assert.IsNotNull(newRole, "Slechts één rol beschikbaar in dropdown; kan wijziging niet testen.");

            selectElement.SelectByText(newRole);
            LogInfo($"Changing role from '{currentRole}' to '{newRole}'.");
            LogSuccess(4, "New role selected in the UI.");

            // Stap 5: Opslaan
            LogStep(5, "Clicking the 'Save' button...");
            userManagementPage.ClickSaveButtonByRow(firstUserRow);
            LogSuccess(5, "Save button clicked.");

            // Stap 6: Alert afhandeling
            LogStep(6, "Waiting for and handling confirmation alert...");
            try
            {
                // Wacht maximaal 5 seconden tot de alert echt verschijnt
                WebDriverWait alertWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IAlert alert = alertWait.Until(d => {
                    try
                    {
                        return d.SwitchTo().Alert();
                    }
                    catch (NoAlertPresentException)
                    {
                        return null;
                    }
                });

                if (alert != null)
                {
                    LogInfo($"Alert detected: {alert.Text}");
                    alert.Accept();
                    LogSuccess(6, "Alert successfully accepted.");
                }
            }
            catch (WebDriverTimeoutException)
            {
                LogInfo("No alert appeared within timeout (acceptable if UI saves silently).");
                LogSuccess(6, "Proceeding without alert.");
            }

            LogSuccess(6, "Alert check completed.");

            RetryVerification(() =>
            {
                LogStep(7, "Refreshing page to synchronize UI with database...");
                driver.Navigate().Refresh();
                wait.Until(d => d.FindElements(By.ClassName("user-row")).Count > 0);

                var userRowsAfter = userManagementPage.GetUserRows();
                var roleDropdownAfter = userManagementPage.GetSelectByRow(userRowsAfter[0]);
                var selectAfter = new SelectElement(roleDropdownAfter);

                // Gebruik Trim() om verborgen spaties te verwijderen
                var userEmail = userRowsAfter[0].FindElement(By.Id("user-email")).Text.Trim();
                LogInfo($"Found email: '{userEmail}' on page");

                // UI Check
                string uiRoleResult = selectAfter.SelectedOption.Text;
                LogInfo($"UI Check: Displayed role is '{uiRoleResult}' (Target: '{newRole}')");
                Assert.AreEqual(newRole, uiRoleResult, "UI mismatch.");

                // Database Check met NULL-beveiliging
                _context.ChangeTracker.Clear();
                var dbUser = _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Email == userEmail) ?? throw new Exception($"Database Check Failed: User with email '{userEmail}' not found in DB.");
                
                if (dbUser.Role == null)
                    throw new Exception($"Database Check Failed: Role object for user '{userEmail}' is null.");

                string dbRoleResult = dbUser.Role.RoleName;
                LogInfo($"Database Check: Persisted role is '{dbRoleResult}'");

                Assert.AreEqual(newRole, dbRoleResult, "Database mismatch.");

            }, maxAttempts: 3, delayMs: 3000);

            LogSuccess(8, "Multi-level verification successful: UI and Database are synchronized.");

            // Stap 9: Database Verificatie (Pas doen als UI klopt of voor diepere analyse)
            LogStep(9, "Performing deep-check in database via AppDbContext...");
            _context.ChangeTracker.Clear();

            // Haal email op uit de EERSTE rij
            var userEmail = userManagementPage.GetUserRows()[0].FindElement(By.Id("user-email")).Text;
            var dbUser = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == userEmail);

            Assert.IsNotNull(dbUser, $"Database Error: User '{userEmail}' not found.");
            LogInfo($"Database Verification: Role in DB for '{userEmail}' is '{dbUser.Role.RoleName}'");

            Assert.AreEqual(newRole, dbUser.Role.RoleName, "Persistence Failure: UI might be wrong, but DB is definitely not updated.");
            LogSuccess(9, "Role change verified in Database.");
        }
    }
}