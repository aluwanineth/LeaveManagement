using AutoMapper;
using LeaveManagement.Application.DTOs.Employee;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.Employee.Queries;

public class GetAllEmployeesQueryHandler(IEmployeeRepositoryAsync employeeRepository, IMapper mapper) : IRequestHandler<GetAllEmployeesQuery, Response<IReadOnlyList<EmployeeResponse>>>
{
    public async Task<Response<IReadOnlyList<EmployeeResponse>>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await employeeRepository.GetAllAsync();
        var employeeResponses = mapper.Map<IReadOnlyList<EmployeeResponse>>(employees);

        return new Response<IReadOnlyList<EmployeeResponse>>(employeeResponses, "Employees retrieved successfully");
    }
}
