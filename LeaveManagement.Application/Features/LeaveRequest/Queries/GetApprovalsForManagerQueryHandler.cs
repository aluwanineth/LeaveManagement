using AutoMapper;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.LeaveRequest.Queries;

public class GetApprovalsForManagerQueryHandler(ILeaveRequestRepositoryAsync leaveRequestRepository, IMapper mapper) : IRequestHandler<GetApprovalsForManagerQuery, Response<IReadOnlyList<LeaveRequestResponse>>>
{
    private readonly ILeaveRequestRepositoryAsync _leaveRequestRepository = leaveRequestRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<Response<IReadOnlyList<LeaveRequestResponse>>> Handle(GetApprovalsForManagerQuery request, CancellationToken cancellationToken)
    {
        var leaveRequests = await _leaveRequestRepository.GetLeaveRequestsForApprovalAsync(request.ManagerId);
        var response = _mapper.Map<IReadOnlyList<LeaveRequestResponse>>(leaveRequests);

        return new Response<IReadOnlyList<LeaveRequestResponse>>(response, "Approval history retrieved successfully");
    }
}
