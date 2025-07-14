using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.BusinessDays.Queries;

public class GetNextBusinessDayQueryHandler(IBusinessDayService businessDayService) : IRequestHandler<GetNextBusinessDayQuery, Response<DateTime>>
{
    public async Task<Response<DateTime>> Handle(GetNextBusinessDayQuery request, CancellationToken cancellationToken)
    {
        var result = await businessDayService.GetNextBusinessDayAsync(request.Date);
        return new Response<DateTime>(result, "Next business day retrieved successfully");
    }
}
