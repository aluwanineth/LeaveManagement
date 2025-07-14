using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Persistence.Contexts;
using LeaveManagement.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace LeaveManagement.Tests.Tests.Repositories;

[TestFixture]
public class LeaveRequestRepositoryTests
{
    private ApplicationDbContext _context;
    private LeaveRequestRepositoryAsync _repository;
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
        _repository = new LeaveRequestRepositoryAsync(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetByEmployeeIdAsync_ExistingRequests_ReturnsRequests()
    {
        // Arrange
        var employee = new Employee
        {
            EmployeeId = 1,
            EmployeeNumber = "EMP001",
            FullName = "Test Employee",
            Email = "test@acme.com",
            EmployeeType = EmployeeType.Employee
        };

        var leaveRequest1 = new LeaveRequest
        {
            EmployeeId = 1,
            StartDate = DateTime.Today.AddDays(10),
            EndDate = DateTime.Today.AddDays(15),
            LeaveType = LeaveType.Annual,
            Status = LeaveStatus.Pending,
            Employee = employee
        };

        var leaveRequest2 = new LeaveRequest
        {
            EmployeeId = 1,
            StartDate = DateTime.Today.AddDays(20),
            EndDate = DateTime.Today.AddDays(25),
            LeaveType = LeaveType.Sick,
            Status = LeaveStatus.Approved,
            Employee = employee
        };

        await _context.Employees.AddAsync(employee);
        await _context.LeaveRequests.AddRangeAsync(leaveRequest1, leaveRequest2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmployeeIdAsync(1);

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Any(lr => lr.LeaveType == LeaveType.Annual), Is.True);
        Assert.That(result.Any(lr => lr.LeaveType == LeaveType.Sick), Is.True);
    }

    [Test]
    public async Task GetPendingRequestsForManagerAsync_HasPendingRequests_ReturnsPendingOnly()
    {
        // Arrange
        await SeedTestData();

        // Act
        var result = await _repository.GetPendingRequestsForManagerAsync(2); // Manager ID

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.First().Status, Is.EqualTo(LeaveStatus.Pending));
    }

    [Test]
    public async Task GetPendingApprovalsAsync_CallsPendingRequestsForManager()
    {
        // Arrange
        await SeedTestData();

        // Act
        var result = await _repository.GetPendingApprovalsAsync(2);

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.First().Status, Is.EqualTo(LeaveStatus.Pending));
    }

    private async Task SeedTestData()
    {
        var manager = new Employee
        {
            EmployeeId = 2,
            EmployeeNumber = "MGR001",
            FullName = "Manager",
            Email = "manager@acme.com",
            EmployeeType = EmployeeType.Manager
        };

        var employee = new Employee
        {
            EmployeeId = 1,
            EmployeeNumber = "EMP001",
            FullName = "Employee",
            Email = "employee@acme.com",
            EmployeeType = EmployeeType.Employee,
            ManagerId = 2
        };

        var pendingRequest = new LeaveRequest
        {
            EmployeeId = 1,
            StartDate = DateTime.Today.AddDays(10),
            EndDate = DateTime.Today.AddDays(15),
            LeaveType = LeaveType.Annual,
            Status = LeaveStatus.Pending,
            Employee = employee
        };

        var approvedRequest = new LeaveRequest
        {
            EmployeeId = 1,
            StartDate = DateTime.Today.AddDays(20),
            EndDate = DateTime.Today.AddDays(25),
            LeaveType = LeaveType.Sick,
            Status = LeaveStatus.Approved,
            Employee = employee
        };

        await _context.Employees.AddRangeAsync(manager, employee);
        await _context.LeaveRequests.AddRangeAsync(pendingRequest, approvedRequest);
        await _context.SaveChangesAsync();
    }
}
