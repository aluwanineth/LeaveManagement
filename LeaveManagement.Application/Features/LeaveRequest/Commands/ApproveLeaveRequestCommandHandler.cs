using AutoMapper;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.LeaveRequest.Commands;

public class ApproveLeaveRequestCommandHandler(
    ILeaveRequestRepositoryAsync leaveRequestRepository,
    IAuthorizationService authorizationService,
    IMapper mapper) : IRequestHandler<ApproveLeaveRequestCommand, Response<LeaveRequestResponse>>
{
    public async Task<Response<LeaveRequestResponse>> Handle(ApproveLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var leaveRequest = await leaveRequestRepository.GetByIdAsync(request.ApprovalRequest.LeaveRequestId);

        if (leaveRequest == null)
        {
            return new Response<LeaveRequestResponse>("Leave request not found");
        }

        var canApprove = await authorizationService.CanUserApproveLeaveRequestAsync(request.ApproverId, leaveRequest.EmployeeId);

        if (!canApprove)
        {
            return new Response<LeaveRequestResponse>("You are not authorized to approve this leave request");
        }

        leaveRequest.Status = request.ApprovalRequest.Status;
        leaveRequest.ApprovedById = request.ApproverId;
        leaveRequest.ApprovedDate = DateTime.Now;
        leaveRequest.ApprovalComments = request.ApprovalRequest.ApprovalComments;

        await leaveRequestRepository.UpdateAsync(leaveRequest);

        var response = mapper.Map<LeaveRequestResponse>(leaveRequest);
        return new Response<LeaveRequestResponse>(response, "Leave request processed successfully");
    }
}