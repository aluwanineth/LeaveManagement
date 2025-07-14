using LeaveManagement.Domain.Enums;

namespace LeaveManagement.Application.DTOs.Employee;

public class EmployeeRequest
{
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? CellphoneNumber { get; set; }
    public EmployeeType EmployeeType { get; set; }
    public int? ManagerId { get; set; }
}
