using LeaveManagement.Application.DTOs.BusinessDays;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.BusinessDays.Queries;

public class GetPublicHolidaysQuery(int year) : IRequest<Response<List<PublicHolidayDto>>>
{
    public int Year { get; set; } = year;
}
