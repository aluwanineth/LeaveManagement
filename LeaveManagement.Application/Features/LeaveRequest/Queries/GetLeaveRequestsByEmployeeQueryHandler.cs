using AutoMapper;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.LeaveRequest.Queries;

public class GetLeaveRequestsByEmployeeQueryHandler(ILeaveRequestRepositoryAsync leaveRequestRepository, IMapper mapper) : IRequestHandler<GetLeaveRequestsByEmployeeQuery, Response<IReadOnlyList<LeaveRequestResponse>>>
{
    public async Task<Response<IReadOnlyList<LeaveRequestResponse>>> Handle(GetLeaveRequestsByEmployeeQuery request, CancellationToken cancellationToken)
    {
        var leaveRequests = await leaveRequestRepository.GetByEmployeeIdAsync(request.EmployeeId);
        var leaveRequestResponses = mapper.Map<IReadOnlyList<LeaveRequestResponse>>(leaveRequests);

        return new Response<IReadOnlyList<LeaveRequestResponse>>(leaveRequestResponses, "Leave requests retrieved successfully");
    }
}
