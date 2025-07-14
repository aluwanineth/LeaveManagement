using LeaveManagement.Domain.Enums;

namespace LeaveManagement.Application.DTOs.LeaveRequest;

public class ApproveLeaveRequest
{
    public int LeaveRequestId { get; set; }
    public LeaveStatus Status { get; set; }
    public string? ApprovalComments { get; set; }
}
