using LeaveManagement.Application.DTOs.Employee;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.Employee.Queries;

public class GetAllEmployeesQuery : IRequest<Response<IReadOnlyList<EmployeeResponse>>>
{
}
