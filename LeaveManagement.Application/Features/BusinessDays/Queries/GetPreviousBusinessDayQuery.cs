using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.BusinessDays.Queries;

public class GetPreviousBusinessDayQuery(DateTime date) : IRequest<Response<DateTime>>
{
    public DateTime Date { get; set; } = date;
}
