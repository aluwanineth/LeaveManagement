using LeaveManagement.Domain.Common;
using LeaveManagement.Domain.Enums;

namespace LeaveManagement.Domain.Entities;

public class LeaveRequest : AuditableBaseEntity
{
    public int LeaveRequestId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LeaveType LeaveType { get; set; }
    public string? Comments { get; set; }
    public LeaveStatus Status { get; set; }
    public int? ApprovedById { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovalComments { get; set; }
    public virtual Employee Employee { get; set; } = null!;
    public virtual Employee? ApprovedBy { get; set; }
}
