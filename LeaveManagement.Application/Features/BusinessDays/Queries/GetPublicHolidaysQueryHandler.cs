using LeaveManagement.Application.DTOs.BusinessDays;
using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.BusinessDays.Queries;

public class GetPublicHolidaysQueryHandler(IBusinessDayService businessDayService) : IRequestHandler<GetPublicHolidaysQuery, Response<List<PublicHolidayDto>>>
{
    public async Task<Response<List<PublicHolidayDto>>> Handle(GetPublicHolidaysQuery request, CancellationToken cancellationToken)
    {
        var holidays = businessDayService.GetSouthAfricanPublicHolidays(request.Year);

        var results =  holidays.Select(h => new PublicHolidayDto
        {
            Date = h.Date,
            Name = h.Name,
            IsObserved = h.Name.Contains("observed")
        }).ToList();

        return new Response<List<PublicHolidayDto>>(results, "Public holidays retrieved successfully");
    }
}
