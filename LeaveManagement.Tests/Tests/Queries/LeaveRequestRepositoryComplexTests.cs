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

namespace LeaveManagement.Tests.Tests.Queries
{
    [TestFixture]
    public class LeaveRequestRepositoryComplexTests
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

            await SeedComplexHierarchy();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetPendingRequestsForManagerAsync_DeepHierarchy_ReturnsAllSubordinates()
        {
            // Act - CEO (ID: 1) should see all pending requests
            var result = await _repository.GetPendingRequestsForManagerAsync(1);

            // Assert - Should see requests from both direct and indirect reports
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(result.Any(r => r.EmployeeId == 3), Is.True); // Direct report
            Assert.That(result.Any(r => r.EmployeeId == 4), Is.True); // Indirect report
        }

        [Test]
        public async Task GetPendingRequestsForManagerAsync_MiddleManager_ReturnsDirectAndIndirectReports()
        {
            // Act - Middle Manager (ID: 2) should see their team's requests
            var result = await _repository.GetPendingRequestsForManagerAsync(2);

            // Assert
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(result.Any(r => r.EmployeeId == 4), Is.True); // Direct report
        }

        [Test]
        public async Task GetByEmployeeIdAsync_MultipleRequestsWithDifferentStatuses_ReturnsAllSorted()
        {
            // Act
            var result = await _repository.GetByEmployeeIdAsync(3);

            // Assert
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(2));
            // Should be ordered by StartDate descending
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.That(result[i].StartDate, Is.GreaterThanOrEqualTo(result[i + 1].StartDate));
            }
        }

        [Test]
        public async Task HasPendingLeaveRequestsAsync_ComplexOverlapScenarios_DetectsCorrectly()
        {
            // Test various overlap scenarios

            // Exact match
            var hasExactMatch = await _repository.HasPendingLeaveRequestsAsync(3,
                DateTime.Today.AddDays(10), DateTime.Today.AddDays(12));
            Assert.That(hasExactMatch, Is.True);

            // Partial overlap - start
            var hasStartOverlap = await _repository.HasPendingLeaveRequestsAsync(3,
                DateTime.Today.AddDays(8), DateTime.Today.AddDays(11));
            Assert.That(hasStartOverlap, Is.True);

            // Partial overlap - end
            var hasEndOverlap = await _repository.HasPendingLeaveRequestsAsync(3,
                DateTime.Today.AddDays(11), DateTime.Today.AddDays(15));
            Assert.That(hasEndOverlap, Is.True);

            // Encompassing
            var hasEncompassing = await _repository.HasPendingLeaveRequestsAsync(3,
                DateTime.Today.AddDays(9), DateTime.Today.AddDays(13));
            Assert.That(hasEncompassing, Is.True);

            // No overlap
            var hasNoOverlap = await _repository.HasPendingLeaveRequestsAsync(3,
                DateTime.Today.AddDays(20), DateTime.Today.AddDays(22));
            Assert.That(hasNoOverlap, Is.False);
        }

        private async Task SeedComplexHierarchy()
        {
            // Create complex organizational hierarchy
            var ceo = new Employee
            {
                EmployeeId = 1,
                EmployeeNumber = "CEO001",
                FullName = "Chief Executive Officer",
                Email = "ceo@company.com",
                EmployeeType = EmployeeType.Manager
            };

            var manager = new Employee
            {
                EmployeeId = 2,
                EmployeeNumber = "MGR001",
                FullName = "Department Manager",
                Email = "manager@company.com",
                EmployeeType = EmployeeType.Manager,
                ManagerId = 1 // Reports to CEO
            };

            var teamLead = new Employee
            {
                EmployeeId = 3,
                EmployeeNumber = "TL001",
                FullName = "Team Lead",
                Email = "teamlead@company.com",
                EmployeeType = EmployeeType.Manager,
                ManagerId = 1 // Reports to CEO
            };

            var developer = new Employee
            {
                EmployeeId = 4,
                EmployeeNumber = "DEV001",
                FullName = "Senior Developer",
                Email = "developer@company.com",
                EmployeeType = EmployeeType.Employee,
                ManagerId = 2 // Reports to Manager
            };

            var juniorDev = new Employee
            {
                EmployeeId = 5,
                EmployeeNumber = "DEV002",
                FullName = "Junior Developer",
                Email = "junior@company.com",
                EmployeeType = EmployeeType.Employee,
                ManagerId = 3 // Reports to Team Lead
            };

            // Create various leave requests with different statuses and dates
            var leaveRequests = new List<LeaveRequest>
        {
            // Team Lead's requests
            new LeaveRequest
            {
                EmployeeId = 3,
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(12),
                LeaveType = LeaveType.Annual,
                Status = LeaveStatus.Pending,
                Comments = "Team Lead annual leave"
            },
            new LeaveRequest
            {
                EmployeeId = 3,
                StartDate = DateTime.Today.AddDays(20),
                EndDate = DateTime.Today.AddDays(22),
                LeaveType = LeaveType.Personal,
                Status = LeaveStatus.Approved,
                Comments = "Team Lead personal leave",
                ApprovedById = 1,
                ApprovedDate = DateTime.Today.AddDays(-1)
            },
            // Senior Developer's requests
            new LeaveRequest
            {
                EmployeeId = 4,
                StartDate = DateTime.Today.AddDays(15),
                EndDate = DateTime.Today.AddDays(17),
                LeaveType = LeaveType.Sick,
                Status = LeaveStatus.Pending,
                Comments = "Senior Dev sick leave"
            },
            new LeaveRequest
            {
                EmployeeId = 4,
                StartDate = DateTime.Today.AddDays(-5),
                EndDate = DateTime.Today.AddDays(-3),
                LeaveType = LeaveType.Annual,
                Status = LeaveStatus.Rejected,
                Comments = "Senior Dev rejected leave",
                ApprovedById = 2,
                ApprovedDate = DateTime.Today.AddDays(-7),
                ApprovalComments = "Project deadline conflict"
            },
            // Junior Developer's requests
            //new LeaveRequest
            //{
            //    EmployeeId = 5,
            //    StartDate = DateTime.Today.AddDays(25),
            //    EndDate = DateTime.Today.AddDays(27),
            //    LeaveType = LeaveType.Training,
            //    Status = LeaveStatus.Pending,
            //    Comments = "Junior Dev training"
            //}
        };

            await _context.Employees.AddRangeAsync(ceo, manager, teamLead, developer, juniorDev);
            await _context.LeaveRequests.AddRangeAsync(leaveRequests);
            await _context.SaveChangesAsync();
        }
    }
}