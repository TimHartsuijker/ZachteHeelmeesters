using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SeleniumTests.Pages
{
    public class UserManagementPage(IWebDriver driver) : BasePage(driver)
    {
        // URL
        public static string Path => "/admin/users";

        public static By SaveButton => By.ClassName("save-btn");

        // Actions
        public void Navigate()
        {
            driver.Navigate().GoToUrl(BaseUrl + Path);
        }

        public ReadOnlyCollection<IWebElement> GetUserRows()
        {
            Wait.Until(d => d.FindElements(By.ClassName("user-row")).Count > 0);
            return driver.FindElements(By.ClassName("user-row"));
        }

        public IWebElement GetSelectByRow(IWebElement row)
        {
            return row.FindElement(By.ClassName("role-select"));
        }

        public void ClickSaveButtonByRow(IWebElement row)
        {
            row.FindElement(SaveButton).Click();
        }
    }
}
