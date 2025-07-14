using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.BusinessDays.Queries;

public class IsBusinessDayQueryHandler(IBusinessDayService businessDayService) : IRequestHandler<IsBusinessDayQuery, Response<bool>>
{
    public async Task<Response<bool>> Handle(IsBusinessDayQuery request, CancellationToken cancellationToken)
    {
        var result = await businessDayService.IsBusinessDayAsync(request.Date);
        return new Response<bool>(result, result ? "The date is a business day." : "The date is not a business day.");
    }
}
