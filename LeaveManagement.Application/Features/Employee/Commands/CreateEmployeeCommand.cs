using LeaveManagement.Application.DTOs.Employee;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.Employee.Commands;

public class CreateEmployeeCommand : IRequest<Response<EmployeeResponse>>
{
    public EmployeeRequest Employee { get; set; } = null!;
}
