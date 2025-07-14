namespace LeaveManagement.Application.Interfaces.Services;

public interface IAuthenticatedUserService
{
    string? UserId { get; }
    string? UserName { get; }
    int? EmployeeId { get; }
}
