using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Persistence.Contexts;
using LeaveManagement.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Tests.Tests.StressLoadTests
{
    [TestFixture]
    public class LeaveRequestStressTests
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
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task CreateMultipleRequests_RapidSuccession_ShouldHandleCorrectly()
        {
            // Arrange
            var employee = new Employee
            {
                EmployeeId = 1,
                EmployeeNumber = "EMP001",
                FullName = "Test Employee",
                Email = "test@acme.com"
            };

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            var tasks = new List<Task<LeaveRequest>>();

            // Act - Create multiple requests simultaneously
            for (int i = 0; i < 10; i++)
            {
                var startDay = i * 5 + 10; // Avoid overlapping dates
                var request = new LeaveRequest
                {
                    EmployeeId = 1,
                    StartDate = DateTime.Today.AddDays(startDay),
                    EndDate = DateTime.Today.AddDays(startDay + 2),
                    LeaveType = LeaveType.Annual,
                    Status = LeaveStatus.Pending,
                    Comments = $"Request {i + 1}",
                    Employee = employee
                };

                tasks.Add(_repository.AddAsync(request));
            }

            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.That(results.Length, Is.EqualTo(10));
            Assert.That(results.All(r => r.LeaveRequestId > 0), Is.True);

            // Verify all requests were created
            var allRequests = await _repository.GetByEmployeeIdAsync(1);
            Assert.That(allRequests.Count, Is.EqualTo(10));
        }

        [Test]
        public async Task BulkStatusUpdates_LargeDataset_ShouldPerformWell()
        {
            // Arrange
            await CreateLargeDataset(100);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act - Update all pending requests to approved
            var pendingRequests = await _repository.GetLeaveRequestsByStatusAsync(LeaveStatus.Pending);

            foreach (var request in pendingRequests)
            {
                request.Status = LeaveStatus.Approved;
                request.ApprovedById = 1;
                request.ApprovedDate = DateTime.Now;
                await _repository.UpdateAsync(request);
            }

            stopwatch.Stop();

            // Assert
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(5000), "Bulk updates should complete within 5 seconds");

            var approvedCount = await _context.LeaveRequests.CountAsync(lr => lr.Status == LeaveStatus.Approved);
            Assert.That(approvedCount, Is.GreaterThan(0));
        }

        private async Task CreateLargeDataset(int requestCount)
        {
            var employees = new List<Employee>();
            var leaveRequests = new List<LeaveRequest>();

            // Create employees
            for (int i = 1; i <= 20; i++)
            {
                employees.Add(new Employee
                {
                    EmployeeId = i,
                    EmployeeNumber = $"EMP{i:D3}",
                    FullName = $"Employee {i}",
                    Email = $"employee{i}@company.com",
                    EmployeeType = EmployeeType.Employee
                });
            }

            // Create leave requests
            for (int i = 1; i <= requestCount; i++)
            {
                var employeeId = (i % 20) + 1; // Distribute across employees
                leaveRequests.Add(new LeaveRequest
                {
                    EmployeeId = employeeId,
                    StartDate = DateTime.Today.AddDays(i),
                    EndDate = DateTime.Today.AddDays(i + 2),
                    LeaveType = (LeaveType)((i % 5) + 1),
                    Status = LeaveStatus.Pending,
                    Comments = $"Request {i}"
                });
            }

            await _context.Employees.AddRangeAsync(employees);
            await _context.LeaveRequests.AddRangeAsync(leaveRequests);
            await _context.SaveChangesAsync();
        }
    }

}
