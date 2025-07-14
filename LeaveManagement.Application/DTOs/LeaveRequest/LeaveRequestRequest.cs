using LeaveManagement.Domain.Enums;

namespace LeaveManagement.Application.DTOs.LeaveRequest;

public class LeaveRequestRequest
{
    public int EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LeaveType LeaveType { get; set; }
    public string? Comments { get; set; }
}
