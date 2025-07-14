namespace LeaveManagement.Application.DTOs.BusinessDays;

public class BusinessDayValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<DateTime> NonBusinessDays { get; set; } = new();
    public int BusinessDaysCount { get; set; }
    public List<PublicHolidayDto> PublicHolidaysInRange { get; set; } = new();
}
