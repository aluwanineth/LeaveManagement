using AutoMapper;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.LeaveRequest.Queries;

public class GetPendingApprovalsQueryHandler(ILeaveRequestRepositoryAsync leaveRequestRepository, IMapper mapper) : IRequestHandler<GetPendingApprovalsQuery, Response<IReadOnlyList<LeaveRequestResponse>>>
{
    public async Task<Response<IReadOnlyList<LeaveRequestResponse>>> Handle(GetPendingApprovalsQuery request, CancellationToken cancellationToken)
    {
        var pendingApprovals = await leaveRequestRepository.GetPendingRequestsForManagerAsync(request.ApproverId);
        var leaveRequestResponses = mapper.Map<IReadOnlyList<LeaveRequestResponse>>(pendingApprovals);

        return new Response<IReadOnlyList<LeaveRequestResponse>>(leaveRequestResponses, "Pending approvals retrieved successfully");
    }
}
