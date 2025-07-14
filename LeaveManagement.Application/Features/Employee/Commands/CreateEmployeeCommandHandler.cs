using AutoMapper;
using LeaveManagement.Application.DTOs.Employee;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.Employee.Commands;

public class CreateEmployeeCommandHandler(IEmployeeRepositoryAsync employeeRepository, IMapper mapper) : IRequestHandler<CreateEmployeeCommand, Response<EmployeeResponse>>
{
    public async Task<Response<EmployeeResponse>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = mapper.Map<Domain.Entities.Employee>(request.Employee);
            var createdEmployee = await employeeRepository.AddAsync(employee);
            var employeeResponse = mapper.Map<EmployeeResponse>(createdEmployee);


            return new Response<EmployeeResponse>(employeeResponse, "Employee created successfully");
        }
        catch (Exception ex)
        {
            return new Response<EmployeeResponse>($"An error occurred while creating the employee, Error: {ex.Message}");
        }
    }
}
