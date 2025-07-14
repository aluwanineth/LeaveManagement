using Asp.Versioning;
using LeaveManagement.Application.DTOs.Employee;
using LeaveManagement.Application.Features.Employee.Commands;
using LeaveManagement.Application.Features.Employee.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers.V1;

[ApiVersion("1.0")]
public class EmployeeController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpGet]
    [Authorize(Roles = "CEO,Manager")]
    public async Task<IActionResult> GetAllEmployees()
    {
        var query = new GetAllEmployeesQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "CEO,Manager")]
    public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequest employeeRequest)
    {
        var command = new CreateEmployeeCommand { Employee = employeeRequest };
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}
