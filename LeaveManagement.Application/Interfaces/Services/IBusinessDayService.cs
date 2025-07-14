using LeaveManagement.Application.Services;

namespace LeaveManagement.Application.Interfaces.Services;

public interface IBusinessDayService
{
    Task<bool> IsBusinessDayAsync(DateTime date);
    Task<DateTime> GetNextBusinessDayAsync(DateTime date);
    Task<DateTime> GetPreviousBusinessDayAsync(DateTime date);
    Task<int> GetBusinessDaysCountAsync(DateTime startDate, DateTime endDate);
    Task<List<DateTime>> GetNonBusinessDaysInRangeAsync(DateTime startDate, DateTime endDate);
    Task<bool> IsDateRangeValidForLeaveAsync(DateTime startDate, DateTime endDate);
    Task<(DateTime adjustedStart, DateTime adjustedEnd)> AdjustDatesToBusinessDaysAsync(DateTime startDate, DateTime endDate);
    List<PublicHolidayInfo> GetSouthAfricanPublicHolidays(int year);
}
