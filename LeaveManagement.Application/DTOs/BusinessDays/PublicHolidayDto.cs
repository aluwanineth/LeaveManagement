namespace LeaveManagement.Application.DTOs.BusinessDays;

public class PublicHolidayDto
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsObserved { get; set; } // For holidays moved due to Sunday rule
}
