using LeaveManagement.Domain.Enums;

namespace LeaveManagement.Application.DTOs.Employee;

public class EmployeeResponse
{
    public int EmployeeId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? CellphoneNumber { get; set; }
    public EmployeeType EmployeeType { get; set; }
    public int? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public DateTime? LastRecordUpdateDate { get; set; }
    public string? LastRecordUpdateUserid { get; set; }
}
