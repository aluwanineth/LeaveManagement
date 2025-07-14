using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.BusinessDays.Queries;

public class GetBusinessDaysCountQuery(DateTime startDate, DateTime endDate) : IRequest<Response<int>>
{
    public DateTime StartDate { get; set; } = startDate;
    public DateTime EndDate { get; set; } = endDate;
}
