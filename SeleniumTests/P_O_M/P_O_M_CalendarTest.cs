using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests.Pages;

public class CalendarTestPage
{
    private readonly string baseUrl = "http://localhost/agenda";
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    // Constructor
    public CalendarTestPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    }

    // Locators
    private By SpecialistDropdownLocator => By.Id("specialist-dropdown");
    private By SearchButtonLocator => By.Id("search-btn");
    private By CalendarViewLocator => By.ClassName("calendar-view");
    private By SpecialistDisplayLocator => By.ClassName("specialist-display");
    private By ErrorMessageLocator => By.ClassName("error-message");
    private By StatusCodeLocator => By.ClassName("status-code");
    private By SlotAvailableLocator => By.ClassName("slot-available");
    private By SlotBookedLocator => By.ClassName("slot-booked");
    private By TimeSlotLocator => By.ClassName("time-slot");

    // Navigation
    public void Navigate()
    {
        _driver.Navigate().GoToUrl(baseUrl);
    }

    // Wait for page to load
    public void WaitForPageLoad()
    {
        _wait.Until(d => {
            try
            {
                var dropdown = d.FindElement(SpecialistDropdownLocator);
                return dropdown.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        });
    }

    // Get dropdown element
    public SelectElement GetSpecialistDropdown()
    {
        var dropdown = _driver.FindElement(SpecialistDropdownLocator);
        var selectElement = new SelectElement(dropdown);
        
        // Wait for options to load
        _wait.Until(d => selectElement.Options.Count > 1);
        
        return selectElement;
    }

    // Get valid specialists (excluding placeholder)
    public List<IWebElement> GetValidSpecialists()
    {
        var selectElement = GetSpecialistDropdown();
        return selectElement.Options
            .Where(o => !string.IsNullOrEmpty(o.GetAttribute("value")) && o.GetAttribute("value") != "0")
            .ToList();
    }

    // Select specialist by ID
    public void SelectSpecialistById(string specialistId)
    {
        var selectElement = GetSpecialistDropdown();
        selectElement.SelectByValue(specialistId);
    }

    // Add invalid specialist option (for testing)
    public void AddInvalidSpecialistOption(string invalidId, string displayName)
    {
        IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
        js.ExecuteScript(@"
            var select = arguments[0];
            var option = document.createElement('option');
            option.value = arguments[1];
            option.text = arguments[2];
            select.appendChild(option);
            select.value = arguments[1];
        ", _driver.FindElement(SpecialistDropdownLocator), invalidId, displayName);
    }

    // Click search button
    public void ClickSearchButton()
    {
        var searchButton = _driver.FindElement(SearchButtonLocator);
        searchButton.Click();
    }

    // Wait for calendar to load
    public void WaitForCalendarToLoad()
    {
        _wait.Until(d => {
            try
            {
                var calendarView = d.FindElement(CalendarViewLocator);
                return calendarView.Displayed;
            }
            catch
            {
                return false;
            }
        });
    }

    // Wait for calendar to update for specific specialist
    public void WaitForCalendarToUpdateForSpecialist(string specialistId)
    {
        _wait.Until(d => {
            try
            {
                var display = d.FindElement(SpecialistDisplayLocator);
                return display.GetAttribute("data-specialist-id") == specialistId;
            }
            catch
            {
                return false;
            }
        });
    }

    // Wait for error message
    public void WaitForErrorMessage()
    {
        _wait.Until(d => {
            try
            {
                var errorMessage = d.FindElement(ErrorMessageLocator);
                return errorMessage.Displayed;
            }
            catch
            {
                return false;
            }
        });
    }

    // Get displayed specialist ID
    public string GetDisplayedSpecialistId()
    {
        var specialistDisplay = _driver.FindElement(SpecialistDisplayLocator);
        return specialistDisplay.GetAttribute("data-specialist-id");
    }

    // Get error message text
    public string GetErrorMessage()
    {
        var errorElement = _driver.FindElement(ErrorMessageLocator);
        return errorElement.Text;
    }

    // Get status code (if displayed)
    public string GetStatusCode()
    {
        var statusElements = _driver.FindElements(StatusCodeLocator);
        if (statusElements.Count > 0 && statusElements[0].Displayed)
        {
            return statusElements[0].Text;
        }
        return null;
    }

    // Get available slots count
    public int GetAvailableSlotsCount()
    {
        return _driver.FindElements(SlotAvailableLocator).Count;
    }

    // Get booked slots count
    public int GetBookedSlotsCount()
    {
        return _driver.FindElements(SlotBookedLocator).Count;
    }

    // Get all time slots
    public IReadOnlyCollection<IWebElement> GetAllTimeSlots()
    {
        return _driver.FindElements(TimeSlotLocator);
    }

    // Check if calendar is visible
    public bool IsCalendarVisible()
    {
        var calendarElements = _driver.FindElements(CalendarViewLocator);
        return calendarElements.Count > 0 && calendarElements[0].Displayed;
    }

    // Check if any time slot is visible
    public bool AreTimeSlotsVisible()
    {
        var timeSlots = _driver.FindElements(TimeSlotLocator);
        return timeSlots.Any(slot => slot.Displayed);
    }

    // === WORKFLOW METHODS (Composite Actions) ===

    // Complete workflow: Select specialist and load their calendar
    public void SelectAndLoadSpecialist(string specialistId)
    {
        SelectSpecialistById(specialistId);
        ClickSearchButton();
        WaitForCalendarToUpdateForSpecialist(specialistId);
    }

    // Verify specialist ID matches expected
    public void VerifyDisplayedSpecialistId(string expectedId)
    {
        string displayedId = GetDisplayedSpecialistId();
        if (displayedId != expectedId)
        {
            throw new Exception($"Wrong specialist displayed. Expected {expectedId}, got {displayedId}");
        }
    }

    // Get specialist availability summary
    public (int available, int booked) GetAvailabilitySummary()
    {
        return (GetAvailableSlotsCount(), GetBookedSlotsCount());
    }

    // Ensure minimum specialist count for testing
    public void EnsureMinimumSpecialistCount(int minimumCount)
    {
        var specialists = GetValidSpecialists();
        if (specialists.Count < minimumCount)
        {
            throw new Exception($"Test requires at least {minimumCount} specialists. Found: {specialists.Count}");
        }
    }
}
