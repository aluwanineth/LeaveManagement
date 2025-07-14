using AutoMapper;
using LeaveManagement.Application.DTOs.Employee;
using LeaveManagement.Application.Features.Employee.Commands;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using NSubstitute;

namespace LeaveManagement.Tests.Tests.Handlers;

[TestFixture]
public class CreateEmployeeCommandHandlerTests
{
    private IEmployeeRepositoryAsync _employeeRepository;
    private IMapper _mapper;
    private CreateEmployeeCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _employeeRepository = Substitute.For<IEmployeeRepositoryAsync>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateEmployeeCommandHandler(_employeeRepository, _mapper);
    }

    [Test]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new CreateEmployeeCommand
        {
            Employee = new EmployeeRequest
            {
                EmployeeNumber = "EMP001",
                FullName = "Test Employee",
                Email = "test@acme.com",
                EmployeeType = EmployeeType.Employee
            }
        };

        var employee = new Employee
        {
            EmployeeId = 1,
            EmployeeNumber = "EMP001",
            FullName = "Test Employee",
            Email = "test@acme.com",
            EmployeeType = EmployeeType.Employee
        };

        var employeeResponse = new EmployeeResponse
        {
            EmployeeId = 1,
            EmployeeNumber = "EMP001",
            FullName = "Test Employee",
            Email = "test@acme.com",
            EmployeeType = EmployeeType.Employee
        };

        _mapper.Map<Employee>(request.Employee).Returns(employee);
        _employeeRepository.AddAsync(employee).Returns(employee);
        _mapper.Map<EmployeeResponse>(employee).Returns(employeeResponse);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.Succeeded, Is.True);
        Assert.That(result.Data, Is.EqualTo(employeeResponse));
        Assert.That(result.Message, Is.EqualTo("Employee created successfully"));

        await _employeeRepository.Received(1).AddAsync(Arg.Any<Employee>());
        _mapper.Received(1).Map<Employee>(request.Employee);
        _mapper.Received(1).Map<EmployeeResponse>(employee);
    }
}