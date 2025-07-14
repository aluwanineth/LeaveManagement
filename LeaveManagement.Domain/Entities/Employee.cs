using LeaveManagement.Domain.Common;
using LeaveManagement.Domain.Enums;

namespace LeaveManagement.Domain.Entities;

public class Employee : AuditableBaseEntity
{
    public int EmployeeId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? CellphoneNumber { get; set; }
    public EmployeeType EmployeeType { get; set; }
    public int? ManagerId { get; set; }
    public virtual Employee? Manager { get; set; }
    public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public virtual ICollection<LeaveRequest> ApprovedLeaveRequests { get; set; } = new List<LeaveRequest>();
}
