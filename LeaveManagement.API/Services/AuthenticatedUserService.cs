using LeaveManagement.Application.Interfaces.Services;
using System.Security.Claims;

namespace LeaveManagement.API.Services;

public class AuthenticatedUserService : IAuthenticatedUserService
{
    public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
    {
        UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        UserName = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

        var employeeIdClaim = httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
        if (int.TryParse(employeeIdClaim, out int empId))
        {
            EmployeeId = empId;
        }
    }

    public string? UserId { get; }
    public string? UserName { get; }
    public int? EmployeeId { get; }
}
