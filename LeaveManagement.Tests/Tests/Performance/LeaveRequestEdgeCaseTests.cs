using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Persistence.Contexts;
using LeaveManagement.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace LeaveManagement.Tests.Tests.Performance;

[TestFixture]
public class LeaveRequestEdgeCaseTests
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
    public async Task GetPendingRequestsForManagerAsync_ManagerWithNoSubordinates_ReturnsEmptyList()
    {
        // Arrange
        var manager = new Employee
        {
            EmployeeId = 1,
            EmployeeNumber = "MGR001",
            FullName = "Manager Without Team",
            Email = "manager@test.com",
            EmployeeType = EmployeeType.Manager
        };

        await _context.Employees.AddAsync(manager);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPendingRequestsForManagerAsync(1);

        // Assert
        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task GetByEmployeeIdAsync_NonExistentEmployee_ReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetByEmployeeIdAsync(999);

        // Assert
        Assert.That(result.Count, Is.EqualTo(0));
    }

    //[Test]
    //public async Task GetByIdAsync_NonExistentLeaveRequest_ThrowsKeyNotFoundException()
    //{
    //    // Act & Assert
    //    var ex = await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
    //    {
    //        await _repository.GetByIdAsync(999);
    //    });
    //}

    [Test]
    public async Task AddAsync_LeaveRequestWithSameDates_ShouldSucceed()
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

        var sameDate = DateTime.Today.AddDays(10);
        var leaveRequest = new LeaveRequest
        {
            EmployeeId = 1,
            StartDate = sameDate,
            EndDate = sameDate,
            LeaveType = LeaveType.Sick,
            Status = LeaveStatus.Pending,
            Employee = employee
        };

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.AddAsync(leaveRequest);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StartDate, Is.EqualTo(result.EndDate));
    }

    [Test]
    public async Task UpdateAsync_ModifyApprovedRequest_ShouldUpdateSuccessfully()
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

        var leaveRequest = new LeaveRequest
        {
            EmployeeId = 1,
            StartDate = DateTime.Today.AddDays(10),
            EndDate = DateTime.Today.AddDays(15),
            LeaveType = LeaveType.Annual,
            Status = LeaveStatus.Approved,
            Employee = employee
        };

        await _context.Employees.AddAsync(employee);
        await _context.LeaveRequests.AddAsync(leaveRequest);
        await _context.SaveChangesAsync();

        // Act
        leaveRequest.ApprovalComments = "Modified approval";
        await _repository.UpdateAsync(leaveRequest);

        // Assert
        var updatedRequest = await _context.LeaveRequests.FindAsync(leaveRequest.LeaveRequestId);
        Assert.That(updatedRequest.ApprovalComments, Is.EqualTo("Modified approval"));
    }

    [Test]
    public async Task DeleteAsync_ExistingLeaveRequest_ShouldRemoveFromDatabase()
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

        var leaveRequest = new LeaveRequest
        {
            LeaveRequestId = 1,
            EmployeeId = 1,
            StartDate = DateTime.Today.AddDays(10),
            EndDate = DateTime.Today.AddDays(15),
            LeaveType = LeaveType.Annual,
            Status = LeaveStatus.Pending,
            Employee = employee
        };

        await _context.Employees.AddAsync(employee);
        await _context.LeaveRequests.AddAsync(leaveRequest);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(1);

        // Assert
        var deletedRequest = await _context.LeaveRequests.FindAsync(1);
        Assert.That(deletedRequest, Is.Null);
    }
}
