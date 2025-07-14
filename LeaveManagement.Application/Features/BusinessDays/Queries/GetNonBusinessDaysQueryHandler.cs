using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.BusinessDays.Queries;

public class GetNonBusinessDaysQueryHandler(IBusinessDayService businessDayService) : IRequestHandler<GetNonBusinessDaysQuery, Response<List<DateTime>>>
{
    public async Task<Response<List<DateTime>>> Handle(GetNonBusinessDaysQuery request, CancellationToken cancellationToken)
    {
        var results = await businessDayService.GetNonBusinessDaysInRangeAsync(request.StartDate, request.EndDate);
        return new Response<List<DateTime>>(results, "Non-business days retrieved successfully");
    }
}
