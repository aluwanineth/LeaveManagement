using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Persistence.Contexts;
using LeaveManagement.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System.Diagnostics;

namespace LeaveManagement.Tests.Tests.Performance;

[TestFixture]
public class RepositoryPerformanceTests
{
    private ApplicationDbContext _context;
    private LeaveRequestRepositoryAsync _repository;
    private IAuthenticatedUserService _authenticatedUserService;

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _authenticatedUserService = Substitute.For<IAuthenticatedUserService>();
        _authenticatedUserService.UserId.Returns("test-user");

        _context = new ApplicationDbContext(options, _authenticatedUserService);
        _repository = new LeaveRequestRepositoryAsync(_context);

        await SeedLargeDataset();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetPendingRequestsForManagerAsync_LargeDataset_PerformanceTest()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = await _repository.GetPendingRequestsForManagerAsync(1);

        // Assert
        stopwatch.Stop();
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(1000), "Query should complete within 1 second");
        Assert.That(result.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetByEmployeeIdAsync_LargeDataset_PerformanceTest()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = await _repository.GetByEmployeeIdAsync(2);

        // Assert
        stopwatch.Stop();
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(500), "Query should complete within 500ms");
        Assert.That(result.Count, Is.GreaterThan(0));
    }

    private async Task SeedLargeDataset()
    {
        var manager = new Employee
        {
            EmployeeId = 1,
            EmployeeNumber = "MGR001",
            FullName = "Manager",
            Email = "manager@test.com",
            EmployeeType = EmployeeType.Manager
        };

        var employees = new List<Employee> { manager };
        var leaveRequests = new List<LeaveRequest>();

        // Create 1000 employees
        for (int i = 2; i <= 1001; i++)
        {
            employees.Add(new Employee
            {
                EmployeeId = i,
                EmployeeNumber = $"EMP{i:D3}",
                FullName = $"Employee {i}",
                Email = $"employee{i}@test.com",
                EmployeeType = EmployeeType.Employee,
                ManagerId = 1
            });
        }

        // Create 5000 leave requests
        for (int i = 1; i <= 5000; i++)
        {
            var employeeId = (i % 1000) + 2; // Distribute across employees
            leaveRequests.Add(new LeaveRequest
            {
                LeaveRequestId = i,
                EmployeeId = employeeId,
                StartDate = DateTime.Today.AddDays(i % 365),
                EndDate = DateTime.Today.AddDays((i % 365) + 5),
                LeaveType = (LeaveType)((i % 5) + 1),
                Status = (LeaveStatus)((i % 4) + 1),
                Comments = $"Leave request {i}"
            });
        }

        await _context.Employees.AddRangeAsync(employees);
        await _context.LeaveRequests.AddRangeAsync(leaveRequests);
        await _context.SaveChangesAsync();
    }
}
