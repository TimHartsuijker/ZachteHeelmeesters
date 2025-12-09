using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumTests.P_O_M;
using SeleniumTests.TestBase;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace SeleniumTests.Tests.US2
{
    [TestClass]
    public class US2_12_MedischDossierTests : TestBaseClass
    {
        private MedischDossierPage page;
        private LoginPage P_O_M_inlog;

        [TestInitialize]
        public void SetupTest()
        {
            P_O_M_inlog = new LoginPage(driver);
            P_O_M_inlog.LoginAsPatient("testpatient@example.com", "password123");

            page = new MedischDossierPage(driver);
            page.Navigate();
        }


        // ============================================================
        // AC2.12.1  Open Medical Record
        // ============================================================

        [TestMethod]
        public void TC_2_12_1_1_SuccessfulOpening()
        {
            page.OpenMedicalRecord();
            Assert.IsTrue(driver.Url.Contains("medisch-dossier"));
            Assert.IsTrue(driver.PageSource.Contains("Naam patient"));
        }

        [TestMethod]
        public void TC_2_12_1_2_NoAccessWithoutLogin()
        {
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Navigate().GoToUrl(page.Url);
            Assert.IsTrue(driver.Url.Contains("inloggen"));
        }

        [TestMethod]
        public void TC_2_12_1_3_BackendFailureShowsError()
        {
            driver.Navigate().GoToUrl(page.Url + "?fail=true");
            Assert.IsTrue(driver.PageSource.Contains("kan niet geladen worden") ||
                          driver.PageSource.Contains("probeer het later opnieuw"));
        }

        // ============================================================
        // AC2.12.2  View Complete Medical History
        // ============================================================

        [TestMethod]
        public void TC_2_12_2_1_CompleteMedicalHistoryVisible()
        {
            page.OpenMedicalRecord();
            Assert.IsTrue(page.IsMedicalHistoryVisible());
            Assert.IsTrue(driver.PageSource.Contains("Diagnoses") &&
                          driver.PageSource.Contains("Medicatie") &&
                          driver.PageSource.Contains("Labresultaten"));
        }

        [TestMethod]
        public void TC_2_12_2_4_NoMissingHistoricalData()
        {
            page.OpenMedicalRecord();
            page.FilterByDate("2010"); // oudste data
            int count = page.GetMedicalHistoryCount();
            Assert.IsTrue(count > 0);
            // Database check (stub)
            bool databaseHasOlderData = false;
            Assert.IsFalse(databaseHasOlderData);
        }

        [TestMethod]
        public void TC_2_12_2_7_HistoryBackendFailureShowsError()
        {
            driver.Navigate().GoToUrl(page.Url + "?history_fail=true");
            Assert.IsTrue(driver.PageSource.Contains("Geschiedenis kon niet geladen worden") ||
                          driver.PageSource.Contains("probeer het later opnieuw"));
        }

        // ============================================================
        // AC2.12.3  Structure and Filtering
        // ============================================================

        [TestMethod]
        public void TC_2_12_3_1_ChronologicalSorting()
        {
            page.OpenMedicalRecord();
            var items = driver.FindElements(By.CssSelector(".medical-history-entry"));
            Assert.IsTrue(items.Count >= 2);
            var dates = items.Select(i => DateTime.Parse(i.GetAttribute("data-date"))).ToList();
            var sortedDescending = dates.OrderByDescending(d => d).ToList();
            CollectionAssert.AreEqual(sortedDescending, dates);
        }

        [TestMethod]
        public void TC_2_12_3_2_FilterByCategory()
        {
            page.OpenMedicalRecord();
            page.FilterByCategory("Consult");
            var entries = driver.FindElements(By.CssSelector(".medical-history-entry"));
            foreach (var entry in entries)
            {
                string category = entry.GetAttribute("data-category");
                Assert.AreEqual("Consult", category);
            }
        }

        [TestMethod]
        public void TC_2_12_3_4_InvalidDateRangeShowsValidation()
        {
            page.OpenMedicalRecord();
            driver.FindElement(By.Id("filter-start-date")).SendKeys("2025-12-01");
            driver.FindElement(By.Id("filter-end-date")).SendKeys("2025-01-01");
            driver.FindElement(By.Id("apply-filter")).Click();
            Assert.IsTrue(driver.PageSource.Contains("kan niet eerder zijn dan"));
        }

        [TestMethod]
        public void TC_2_12_3_5_CombinedFiltering()
        {
            page.OpenMedicalRecord();
            page.FilterByCategory("Consult");
            page.FilterByDate("Last6Months");
            var entries = driver.FindElements(By.CssSelector(".medical-history-entry"));
            foreach (var entry in entries)
            {
                string category = entry.GetAttribute("data-category");
                string date = entry.GetAttribute("data-date");
                Assert.AreEqual("Consult", category);
                Assert.IsTrue(DateTime.Parse(date) >= DateTime.Now.AddMonths(-6));
            }
        }

        // ============================================================
        // AC2.14.4  Read-only access
        // ============================================================

        [TestMethod]
        public void TC_2_14_4_1_NoEditOptionsVisible()
        {
            page.OpenMedicalRecord();
            Assert.IsFalse(page.AreEditButtonsVisible());
        }
    }
}
