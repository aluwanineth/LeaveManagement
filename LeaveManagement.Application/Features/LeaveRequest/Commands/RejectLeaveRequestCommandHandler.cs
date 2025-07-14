using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Application.Wrappers;
using LeaveManagement.Domain.Enums;
using MediatR;

namespace LeaveManagement.Application.Features.LeaveRequest.Commands;

public class RejectLeaveRequestCommandHandler : IRequestHandler<RejectLeaveRequestCommand, Response<string>>
{
    private readonly ILeaveRequestRepositoryAsync _leaveRequestRepository;
    private readonly IAuthorizationService _authorizationService;

    public RejectLeaveRequestCommandHandler(
        ILeaveRequestRepositoryAsync leaveRequestRepository,
        IAuthorizationService authorizationService)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _authorizationService = authorizationService;
    }

    public async Task<Response<string>> Handle(RejectLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get the leave request with details
            var leaveRequest = await _leaveRequestRepository.GetLeaveRequestWithDetailsAsync(request.LeaveRequestId);

            if (leaveRequest == null)
            {
                return new Response<string>("Leave request not found");
            }

            // Check if the leave request is in pending status
            if (leaveRequest.Status != LeaveStatus.Pending)
            {
                return new Response<string>("Only pending leave requests can be rejected");
            }

            // Verify that the rejector has authority to reject this request using your existing authorization service
            var hasAuthority = await _authorizationService.CanUserApproveLeaveRequestAsync(request.RejectedById, leaveRequest.EmployeeId);
            if (!hasAuthority)
            {
                return new Response<string>("You don't have authority to reject this leave request");
            }

            // Validate rejection comments
            if (string.IsNullOrWhiteSpace(request.RejectionComments))
            {
                return new Response<string>("Rejection comments are required");
            }

            // Update the leave request
            leaveRequest.Status = LeaveStatus.Rejected;
            leaveRequest.ApprovedById = request.RejectedById;
            leaveRequest.ApprovedDate = DateTime.Now;
            leaveRequest.ApprovalComments = request.RejectionComments;

            await _leaveRequestRepository.UpdateAsync(leaveRequest);

            return new Response<string>("Leave request rejected successfully",
                $"Leave request for {leaveRequest.Employee?.FullName} from {leaveRequest.StartDate:yyyy-MM-dd} to {leaveRequest.EndDate:yyyy-MM-dd} has been rejected");
        }
        catch (Exception ex)
        {
            return new Response<string>($"An error occurred while rejecting the leave request: {ex.Message}");
        }
    }
}
