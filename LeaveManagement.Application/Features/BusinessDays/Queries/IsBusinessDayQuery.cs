using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.BusinessDays.Queries;

public class IsBusinessDayQuery(DateTime date) : IRequest<Response<bool>>
{
    public DateTime Date { get; set; } = date;
}
