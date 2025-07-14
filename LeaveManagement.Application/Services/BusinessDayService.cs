using LeaveManagement.Application.Interfaces.Services;

namespace LeaveManagement.Application.Services;

public class BusinessDayService : IBusinessDayService
{
    public async Task<bool> IsBusinessDayAsync(DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            return false;
        }

        return !IsSouthAfricanPublicHoliday(date);
    }

    public async Task<DateTime> GetNextBusinessDayAsync(DateTime date)
    {
        var nextDay = date.AddDays(1);

        while (!await IsBusinessDayAsync(nextDay))
        {
            nextDay = nextDay.AddDays(1);
        }

        return nextDay;
    }

    public async Task<DateTime> GetPreviousBusinessDayAsync(DateTime date)
    {
        var previousDay = date.AddDays(-1);

        while (!await IsBusinessDayAsync(previousDay))
        {
            previousDay = previousDay.AddDays(-1);
        }

        return previousDay;
    }

    public async Task<int> GetBusinessDaysCountAsync(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
        {
            throw new ArgumentException("Start date must be before or equal to end date");
        }

        var businessDaysCount = 0;
        var currentDate = startDate;

        while (currentDate <= endDate)
        {
            if (await IsBusinessDayAsync(currentDate))
            {
                businessDaysCount++;
            }
            currentDate = currentDate.AddDays(1);
        }

        return businessDaysCount;
    }

    public async Task<List<DateTime>> GetNonBusinessDaysInRangeAsync(DateTime startDate, DateTime endDate)
    {
        var nonBusinessDays = new List<DateTime>();
        var currentDate = startDate;

        while (currentDate <= endDate)
        {
            if (!await IsBusinessDayAsync(currentDate))
            {
                nonBusinessDays.Add(currentDate);
            }
            currentDate = currentDate.AddDays(1);
        }

        return nonBusinessDays;
    }

    public async Task<bool> IsDateRangeValidForLeaveAsync(DateTime startDate, DateTime endDate)
    {
        if (!await IsBusinessDayAsync(startDate) || !await IsBusinessDayAsync(endDate))
        {
            return false;
        }

        return await GetBusinessDaysCountAsync(startDate, endDate) > 0;
    }

    public async Task<(DateTime adjustedStart, DateTime adjustedEnd)> AdjustDatesToBusinessDaysAsync(DateTime startDate, DateTime endDate)
    {
        var adjustedStart = startDate;
        var adjustedEnd = endDate;

        if (!await IsBusinessDayAsync(adjustedStart))
        {
            adjustedStart = await GetNextBusinessDayAsync(adjustedStart);
        }

        if (!await IsBusinessDayAsync(adjustedEnd))
        {
            adjustedEnd = await GetPreviousBusinessDayAsync(adjustedEnd);
        }

        return (adjustedStart, adjustedEnd);
    }

    public List<PublicHolidayInfo> GetSouthAfricanPublicHolidays(int year)
    {
        var holidays = new List<PublicHolidayInfo>();

        holidays.Add(new PublicHolidayInfo(new DateTime(year, 1, 1), "New Year's Day"));
        holidays.Add(new PublicHolidayInfo(new DateTime(year, 3, 21), "Human Rights Day"));
        holidays.Add(new PublicHolidayInfo(new DateTime(year, 4, 27), "Freedom Day"));
        holidays.Add(new PublicHolidayInfo(new DateTime(year, 5, 1), "Workers' Day"));
        holidays.Add(new PublicHolidayInfo(new DateTime(year, 6, 16), "Youth Day"));
        holidays.Add(new PublicHolidayInfo(new DateTime(year, 8, 9), "National Women's Day"));
        holidays.Add(new PublicHolidayInfo(new DateTime(year, 9, 24), "Heritage Day"));
        holidays.Add(new PublicHolidayInfo(new DateTime(year, 12, 16), "Day of Reconciliation"));
        holidays.Add(new PublicHolidayInfo(new DateTime(year, 12, 25), "Christmas Day"));
        holidays.Add(new PublicHolidayInfo(new DateTime(year, 12, 26), "Day of Goodwill"));

        var easter = CalculateEaster(year);
        var goodFriday = easter.AddDays(-2);
        var familyDay = easter.AddDays(1);

        holidays.Add(new PublicHolidayInfo(goodFriday, "Good Friday"));
        holidays.Add(new PublicHolidayInfo(familyDay, "Family Day"));

        var adjustedHolidays = new List<PublicHolidayInfo>();
        foreach (var holiday in holidays)
        {
            adjustedHolidays.Add(holiday);

            if (holiday.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                var mondayHoliday = new PublicHolidayInfo(
                    holiday.Date.AddDays(1),
                    $"Public holiday {holiday.Name} observed"
                );
                adjustedHolidays.Add(mondayHoliday);
            }
        }

        return adjustedHolidays.OrderBy(h => h.Date).ToList();
    }

    private bool IsSouthAfricanPublicHoliday(DateTime date)
    {
        var holidays = GetSouthAfricanPublicHolidays(date.Year);
        return holidays.Any(h => h.Date.Date == date.Date);
    }

    private DateTime CalculateEaster(int year)
    {
        int a = year % 19;
        int b = year / 100;
        int c = year % 100;
        int d = b / 4;
        int e = b % 4;
        int f = (b + 8) / 25;
        int g = (b - f + 1) / 3;
        int h = (19 * a + b - d - g + 15) % 30;
        int i = c / 4;
        int k = c % 4;
        int l = (32 + 2 * e + 2 * i - h - k) % 7;
        int m = (a + 11 * h + 22 * l) / 451;
        int month = (h + l - 7 * m + 114) / 31;
        int day = ((h + l - 7 * m + 114) % 31) + 1;

        return new DateTime(year, month, day);
    }
}

public class PublicHolidayInfo
{
    public DateTime Date { get; set; }
    public string Name { get; set; }

    public PublicHolidayInfo(DateTime date, string name)
    {
        Date = date;
        Name = name;
    }
}