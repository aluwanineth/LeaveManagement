using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.LeaveRequest.Commands;

public class RejectLeaveRequestCommand : IRequest<Response<string>>
{
    public int LeaveRequestId { get; set; }
    public int RejectedById { get; set; }
    public string RejectionComments { get; set; } = string.Empty;
}
