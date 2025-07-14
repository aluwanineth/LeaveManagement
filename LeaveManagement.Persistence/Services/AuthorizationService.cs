using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Domain.Enums;

namespace LeaveManagement.Persistence.Services;

public class AuthorizationService(IEmployeeRepositoryAsync employeeRepository) : IAuthorizationService
{
    public async Task<bool> CanUserApproveLeaveRequestAsync(int approverId, int employeeId)
    {
        var employee = await employeeRepository.GetByIdAsync(employeeId);
        if (employee == null) return false;

        var approver = await employeeRepository.GetByIdAsync(approverId);
        if (approver == null) return false;

        if (approver.EmployeeType == EmployeeType.CEO) return true;

        if (employee.ManagerId == approverId) return true;

        if (approver.EmployeeType == EmployeeType.Manager && employee.ManagerId.HasValue)
        {
            var employeeManager = await employeeRepository.GetByIdAsync(employee.ManagerId.Value);
            return employeeManager?.ManagerId == approverId;
        }

        return false;
    }

    public async Task<bool> IsUserManagerOfEmployeeAsync(int managerId, int employeeId)
    {
        var employee = await employeeRepository.GetByIdAsync(employeeId);
        return employee?.ManagerId == managerId;
    }
}
