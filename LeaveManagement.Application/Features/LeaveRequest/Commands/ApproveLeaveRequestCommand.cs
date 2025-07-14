using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.LeaveRequest.Commands;

public class ApproveLeaveRequestCommand : IRequest<Response<LeaveRequestResponse>>
{
    public ApproveLeaveRequest ApprovalRequest { get; set; } = null!;
    public int ApproverId { get; set; }
}
