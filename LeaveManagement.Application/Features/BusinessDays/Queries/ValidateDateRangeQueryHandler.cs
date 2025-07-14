using LeaveManagement.Application.DTOs.BusinessDays;
using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.BusinessDays.Queries;

public class ValidateDateRangeQueryHandler(IBusinessDayService businessDayService) : IRequestHandler<ValidateDateRangeQuery, Response<BusinessDayValidationResult>>
{
    public async Task<Response<BusinessDayValidationResult>> Handle(ValidateDateRangeQuery request, CancellationToken cancellationToken)
    {
        var result = new BusinessDayValidationResult
        {
            IsValid = true,
            Errors = new List<string>(),
            NonBusinessDays = new List<DateTime>(),
            BusinessDaysCount = 0,
            PublicHolidaysInRange = new List<PublicHolidayDto>()
        };

        if (request.StartDate > request.EndDate)
        {
            result.IsValid = false;
            result.Errors.Add("Start date must be before or equal to end date");
            return new Response<BusinessDayValidationResult>(result);
        }

        var isStartBusinessDay = await businessDayService.IsBusinessDayAsync(request.StartDate);
        var isEndBusinessDay = await businessDayService.IsBusinessDayAsync(request.EndDate);

        if (!isStartBusinessDay)
        {
            result.IsValid = false;
            result.Errors.Add("Start date must be a business day (Monday-Friday, excluding public holidays)");
        }

        if (!isEndBusinessDay)
        {
            result.IsValid = false;
            result.Errors.Add("End date must be a business day (Monday-Friday, excluding public holidays)");
        }

        result.NonBusinessDays = await businessDayService.GetNonBusinessDaysInRangeAsync(request.StartDate, request.EndDate);

        result.BusinessDaysCount = await businessDayService.GetBusinessDaysCountAsync(request.StartDate, request.EndDate);

        var startYear = request.StartDate.Year;
        var endYear = request.EndDate.Year;
        var publicHolidaysInRange = new List<PublicHolidayDto>();

        for (int year = startYear; year <= endYear; year++)
        {
            var yearHolidays = businessDayService.GetSouthAfricanPublicHolidays(year)
                .Where(h => h.Date >= request.StartDate.Date && h.Date <= request.EndDate.Date)
                .Select(h => new PublicHolidayDto
                {
                    Date = h.Date,
                    Name = h.Name,
                    IsObserved = h.Name.Contains("observed")
                });

            publicHolidaysInRange.AddRange(yearHolidays);
        }

        result.PublicHolidaysInRange = publicHolidaysInRange.OrderBy(h => h.Date).ToList();

        if (result.BusinessDaysCount == 0)
        {
            result.IsValid = false;
            result.Errors.Add("Date range must contain at least one business day");
        }

        return new Response<BusinessDayValidationResult>(result);
    }
}
