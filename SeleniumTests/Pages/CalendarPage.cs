using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests.Pages;

public class CalendarPage(IWebDriver driver) : BasePage(driver)
{
    private static string Path => "/agenda";

    // Locators: Gebruik 'static By' voor betere performance en leesbaarheid
    private static By SpecialistDropdown => By.Id("specialist-dropdown");
    private static By SearchButton => By.Id("search-btn");
    private static By CalendarView => By.ClassName("calendar-view");
    private static By SpecialistDisplay => By.ClassName("specialist-display");
    private static By ErrorMessage => By.ClassName("error-message");
    private static By StatusCode => By.ClassName("status-code");
    private static By SlotAvailable => By.ClassName("slot-available");
    private static By SlotBooked => By.ClassName("slot-booked");
    private static By TimeSlot => By.ClassName("time-slot");

    // Navigation
    public void Navigate() => NavigateTo(BaseUrl + Path);

    // Wachten op pagina: Gebruikt nu de generieke WaitForElement van de BasePage
    public void WaitForPageLoad() => WaitForElement(SpecialistDropdown);

    public SelectElement GetSpecialistDropdown()
    {
        // Wacht via BasePage helper tot de dropdown gevuld is (meer dan alleen de placeholder)
        WaitForDropdownToPopulate(SpecialistDropdown);
        return new SelectElement(Driver.FindElement(SpecialistDropdown));
    }

    public List<IWebElement> GetValidSpecialists()
    {
        return GetSpecialistDropdown().Options
            .Where(o => !string.IsNullOrEmpty(o.GetAttribute("value")) && o.GetAttribute("value") != "0")
            .ToList();
    }

    public void SelectSpecialistById(string specialistId)
    {
        GetSpecialistDropdown().SelectByValue(specialistId);
    }

    public void AddInvalidSpecialistOption(string invalidId, string displayName)
    {
        // Gebruik de ExecuteJs helper van de BasePage
        ExecuteJs(@"
            var select = arguments[0];
            var option = document.createElement('option');
            option.value = arguments[1];
            option.text = arguments[2];
            select.appendChild(option);
            select.value = arguments[1];",
            Driver.FindElement(SpecialistDropdown), invalidId, displayName);
    }

    public void ClickSearchButton() => Click(SearchButton);

    // === Wacht Logica (Sterk vereenvoudigd via BasePage/Wait) ===

    public void WaitForCalendarToLoad() => WaitForElement(CalendarView);

    public void WaitForCalendarToUpdateForSpecialist(string specialistId)
    {
        Wait.Until(d => d.FindElement(SpecialistDisplay).GetAttribute("data-specialist-id") == specialistId);
    }

    public void WaitForErrorMessage() => WaitForElement(ErrorMessage);

    // === Getters & Checks ===

    public string GetDisplayedSpecialistId() => Driver.FindElement(SpecialistDisplay).GetAttribute("data-specialist-id");

    public string GetErrorMessage() => GetText(ErrorMessage);

    public string GetStatusCode() => IsElementDisplayed(StatusCode) ? GetText(StatusCode) : null;

    public int GetAvailableSlotsCount() => Driver.FindElements(SlotAvailable).Count;

    public int GetBookedSlotsCount() => Driver.FindElements(SlotBooked).Count;

    public bool IsCalendarVisible() => IsElementDisplayed(CalendarView);

    public bool AreTimeSlotsVisible() => Driver.FindElements(TimeSlot).Any(slot => slot.Displayed);

    // === WORKFLOWS ===

    public void SelectAndLoadSpecialist(string specialistId)
    {
        SelectSpecialistById(specialistId);
        ClickSearchButton();
        WaitForCalendarToUpdateForSpecialist(specialistId);
    }

    public (int available, int booked) GetAvailabilitySummary() => (GetAvailableSlotsCount(), GetBookedSlotsCount());
}