namespace LeaveManagement.Application.Interfaces.Services;

public interface IAuthorizationService
{
    Task<bool> CanUserApproveLeaveRequestAsync(int approverId, int employeeId);
    Task<bool> IsUserManagerOfEmployeeAsync(int managerId, int employeeId);
}
