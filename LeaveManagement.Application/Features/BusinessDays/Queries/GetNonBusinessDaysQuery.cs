using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.BusinessDays.Queries;

public class GetNonBusinessDaysQuery(DateTime startDate, DateTime endDate) : IRequest<Response<List<DateTime>>>
{
    public DateTime StartDate { get; set; } = startDate;
    public DateTime EndDate { get; set; } = endDate;
}
