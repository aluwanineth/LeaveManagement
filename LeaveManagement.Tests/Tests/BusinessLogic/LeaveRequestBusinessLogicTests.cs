using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Tests.Tests.BusinessLogic
{
    [TestFixture]
    public class LeaveRequestBusinessLogicTests
    {
        [Test]
        public void LeaveRequest_MultipleStatusTransitions_ShouldTrackCorrectly()
        {
            // Arrange
            var leaveRequest = new LeaveRequest
            {
                LeaveRequestId = 1,
                EmployeeId = 1,
                Status = LeaveStatus.Pending,
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(15)
            };

            // Act - First approval
            leaveRequest.Status = LeaveStatus.Approved;
            leaveRequest.ApprovedById = 2;
            leaveRequest.ApprovedDate = DateTime.Now;
            leaveRequest.ApprovalComments = "Initially approved";

            // Act - Later rejection (business rule allowing status change)
            leaveRequest.Status = LeaveStatus.Rejected;
            leaveRequest.ApprovalComments = "Changed to rejected due to coverage issues";

            // Assert
            Assert.That(leaveRequest.Status, Is.EqualTo(LeaveStatus.Rejected));
            Assert.That(leaveRequest.ApprovedById, Is.EqualTo(2));
            Assert.That(leaveRequest.ApprovalComments, Does.Contain("coverage issues"));
        }

        [Test]
        public void LeaveRequest_CalculateDuration_ShouldReturnCorrectDays()
        {
            // Arrange
            var startDate = new DateTime(2024, 6, 10); // Monday
            var endDate = new DateTime(2024, 6, 14);   // Friday

            var leaveRequest = new LeaveRequest
            {
                StartDate = startDate,
                EndDate = endDate
            };

            // Act
            var duration = (leaveRequest.EndDate - leaveRequest.StartDate).Days + 1;

            // Assert
            Assert.That(duration, Is.EqualTo(5)); // 5 working days
        }

        [Test]
        public void LeaveRequest_WeekendLeave_ShouldBeValid()
        {
            // Arrange
            var saturday = new DateTime(2025, 7, 19); // Specific Saturday
            var sunday = saturday.AddDays(1);

            var leaveRequest = new LeaveRequest
            {
                StartDate = saturday,
                EndDate = sunday,
                LeaveType = LeaveType.Personal
            };

            // Assert
            Assert.That(leaveRequest.StartDate.DayOfWeek, Is.EqualTo(DayOfWeek.Saturday));
            Assert.That(leaveRequest.EndDate.DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
            Assert.That(leaveRequest.LeaveType, Is.EqualTo(LeaveType.Personal));
        }
    }
}
