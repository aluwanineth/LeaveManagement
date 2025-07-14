using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.BusinessDays.Queries;

public class GetBusinessDaysCountQueryHandler(IBusinessDayService businessDayService) : IRequestHandler<GetBusinessDaysCountQuery, Response<int>>
{
    public async Task<Response<int>> Handle(GetBusinessDaysCountQuery request, CancellationToken cancellationToken)
    {
        var result = await businessDayService.GetBusinessDaysCountAsync(request.StartDate, request.EndDate);
        return new Response<int>(result, "Business days count retrieved successfully");
    }
}
