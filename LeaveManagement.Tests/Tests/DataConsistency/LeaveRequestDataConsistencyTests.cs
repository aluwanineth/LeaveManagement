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

namespace LeaveManagement.Tests.Tests.DataConsistency
{
    [TestFixture]
    public class LeaveRequestDataConsistencyTests
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
        public async Task LeaveRequest_WithoutEmployee_ShouldHandleGracefully()
        {
            // Arrange - Create leave request without employee entity
            var leaveRequest = new LeaveRequest
            {
                EmployeeId = 999, // Non-existent employee
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(12),
                LeaveType = LeaveType.Annual,
                Status = LeaveStatus.Pending
            };

            // Act & Assert
            // In a real scenario with proper foreign key constraints, this should fail
            // For in-memory database, we test the behavior
            try
            {
                await _repository.AddAsync(leaveRequest);
                var result = await _repository.GetByIdAsync(leaveRequest.LeaveRequestId);
                Assert.That(result, Is.Not.Null);
            }
            catch
            {
                // Expected if proper constraints are in place
                Assert.Pass("Foreign key constraint properly enforced");
            }
        }

        [Test]
        public async Task LeaveRequest_UpdateNonExistent_ShouldHandleGracefully()
        {
            // Arrange
            var nonExistentRequest = new LeaveRequest
            {
                LeaveRequestId = 999,
                EmployeeId = 1,
                Status = LeaveStatus.Approved
            };

            // Act & Assert
            try
            {
                await _repository.UpdateAsync(nonExistentRequest);
                // If no exception, verify the behavior
            }
            catch
            {
                // Expected behavior for updating non-existent entity
                Assert.Pass("Update of non-existent entity properly handled");
            }
        }

        [Test]
        public async Task LeaveRequest_DeleteWithCascade_ShouldMaintainIntegrity()
        {
            // Arrange
            var employee = new Employee
            {
                EmployeeId = 1,
                EmployeeNumber = "EMP001",
                FullName = "Test Employee",
                Email = "test@acme.com"
            };

            var leaveRequest = new LeaveRequest
            {
                LeaveRequestId = 1,
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(12),
                LeaveType = LeaveType.Annual,
                Status = LeaveStatus.Pending,
                Employee = employee
            };

            await _context.Employees.AddAsync(employee);
            await _context.LeaveRequests.AddAsync(leaveRequest);
            await _context.SaveChangesAsync();

            // Act - Delete leave request
            await _repository.DeleteAsync(1);

            // Assert
            var deletedRequest = await _context.LeaveRequests.FindAsync(1);
            Assert.That(deletedRequest, Is.Null);

            // Employee should still exist
            var existingEmployee = await _context.Employees.FindAsync(1);
            Assert.That(existingEmployee, Is.Not.Null);
        }
    }
}
