using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Persistence.Contexts;
using LeaveManagement.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace LeaveManagement.Tests.Tests.Repositories;

[TestFixture]
public class EmployeeRepositoryTests
{
    private ApplicationDbContext _context;
    private EmployeeRepositoryAsync _repository;
    private IAuthenticatedUserService _authenticatedUserService;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _authenticatedUserService = Substitute.For<IAuthenticatedUserService>();
        _authenticatedUserService.UserId.Returns("test-user");

        _context = new ApplicationDbContext(options, _authenticatedUserService);
        _repository = new EmployeeRepositoryAsync(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetByEmployeeNumberAsync_ExistingEmployee_ReturnsEmployee()
    {
        // Arrange
        var employee = new Employee
        {
            EmployeeNumber = "EMP001",
            FullName = "Test Employee",
            Email = "test@acme.com",
            EmployeeType = EmployeeType.Employee
        };

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmployeeNumberAsync("EMP001");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.EmployeeNumber, Is.EqualTo("EMP001"));
        Assert.That(result.FullName, Is.EqualTo("Test Employee"));
    }

    [Test]
    public async Task GetByEmployeeNumberAsync_NonExistingEmployee_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByEmployeeNumberAsync("NONEXISTENT");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetSubordinatesAsync_ManagerWithSubordinates_ReturnsSubordinates()
    {
        // Arrange
        var manager = new Employee
        {
            EmployeeId = 1,
            EmployeeNumber = "MGR001",
            FullName = "Manager",
            Email = "manager@acme.com",
            EmployeeType = EmployeeType.Manager
        };

        var subordinate1 = new Employee
        {
            EmployeeId = 2,
            EmployeeNumber = "EMP001",
            FullName = "Employee 1",
            Email = "emp1@acme.com",
            EmployeeType = EmployeeType.Employee,
            ManagerId = 1
        };

        var subordinate2 = new Employee
        {
            EmployeeId = 3,
            EmployeeNumber = "EMP002",
            FullName = "Employee 2",
            Email = "emp2@acme.com",
            EmployeeType = EmployeeType.Employee,
            ManagerId = 1
        };

        await _context.Employees.AddRangeAsync(manager, subordinate1, subordinate2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetSubordinatesAsync(1);

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Any(e => e.EmployeeNumber == "EMP001"), Is.True);
        Assert.That(result.Any(e => e.EmployeeNumber == "EMP002"), Is.True);
    }

    [Test]
    public async Task AddAsync_ValidEmployee_ReturnsEmployee()
    {
        // Arrange
        var employee = new Employee
        {
            EmployeeNumber = "EMP001",
            FullName = "New Employee",
            Email = "new@acme.com",
            EmployeeType = EmployeeType.Employee
        };

        // Act
        var result = await _repository.AddAsync(employee);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.EmployeeId, Is.GreaterThan(0));
        Assert.That(result.EmployeeNumber, Is.EqualTo("EMP001"));

        var savedEmployee = await _context.Employees.FindAsync(result.EmployeeId);
        Assert.That(savedEmployee, Is.Not.Null);
    }
}
