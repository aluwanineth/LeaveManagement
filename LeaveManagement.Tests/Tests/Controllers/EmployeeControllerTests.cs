using LeaveManagement.API.Controllers.V1;
using LeaveManagement.Application.DTOs.Employee;
using LeaveManagement.Application.Features.Employee.Commands;
using LeaveManagement.Application.Features.Employee.Queries;
using LeaveManagement.Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace LeaveManagement.Tests.Tests.Controllers;

[TestFixture]
public class EmployeeControllerTests
{
    private IMediator _mediator;
    private EmployeeController _controller;

    [SetUp]
    public void SetUp()
    {
        _mediator = Substitute.For<IMediator>();
        _controller = new EmployeeController(_mediator);
    }

    [Test]
    public async Task GetAllEmployees_ReturnsOkResult()
    {
        // Arrange
        var expectedResponse = new Response<IReadOnlyList<EmployeeResponse>>(new List<EmployeeResponse>());
        _mediator.Send(Arg.Any<GetAllEmployeesQuery>()).Returns(expectedResponse);

        // Act
        var result = await _controller.GetAllEmployees();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        await _mediator.Received(1).Send(Arg.Any<GetAllEmployeesQuery>());
    }

    [Test]
    public async Task CreateEmployee_ReturnsOkResult()
    {
        // Arrange
        var employeeRequest = new EmployeeRequest
        {
            FullName = "Test Employee",
            Email = "test@example.com",
            EmployeeNumber = "EMP001"
        };
        var expectedResponse = new Response<EmployeeResponse>(new EmployeeResponse());
        _mediator.Send(Arg.Any<CreateEmployeeCommand>()).Returns(expectedResponse);

        // Act
        var result = await _controller.CreateEmployee(employeeRequest);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        await _mediator.Received(1).Send(Arg.Any<CreateEmployeeCommand>());
    }
}
