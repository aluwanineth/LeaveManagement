using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.BusinessDays.Queries;

public class GetPreviousBusinessDayQueryHandler(IBusinessDayService businessDayService) : IRequestHandler<GetPreviousBusinessDayQuery, Response<DateTime>>
{
    private readonly IBusinessDayService _businessDayService = businessDayService;

    public async Task<Response<DateTime>> Handle(GetPreviousBusinessDayQuery request, CancellationToken cancellationToken)
    {
        var result = await _businessDayService.GetPreviousBusinessDayAsync(request.Date);
        return new Response<DateTime>(result, "Previous business day retrieved successfully");
    }
}
