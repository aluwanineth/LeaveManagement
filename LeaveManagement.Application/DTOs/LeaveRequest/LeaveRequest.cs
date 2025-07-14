using LeaveManagement.Domain.Enums;

namespace LeaveManagement.Application.DTOs.LeaveRequest;

public class LeaveRequestResponse
{
    public int LeaveRequestId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LeaveType LeaveType { get; set; }
    public string? Comments { get; set; }
    public LeaveStatus Status { get; set; }
    public int? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovalComments { get; set; }
    public DateTime? LastRecordUpdateDate { get; set; }
    public string? LastRecordUpdateUserid { get; set; }
}
