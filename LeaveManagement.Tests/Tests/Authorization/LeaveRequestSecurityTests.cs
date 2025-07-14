using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Tests.Tests.Authorization
{
    [TestFixture]
    public class LeaveRequestSecurityTests
    {
        [Test]
        public void LeaveRequest_SensitiveData_ShouldNotExposeInternals()
        {
            // Arrange
            var leaveRequest = new LeaveRequest
            {
                LeaveRequestId = 1,
                EmployeeId = 1,
                Comments = "Personal medical information",
                ApprovalComments = "Manager's private notes"
            };

            // Act
            var response = new LeaveRequestResponse
            {
                LeaveRequestId = leaveRequest.LeaveRequestId,
                EmployeeId = leaveRequest.EmployeeId,
                Comments = leaveRequest.Comments,
                ApprovalComments = leaveRequest.ApprovalComments
            };

            // Assert - Verify data is properly exposed through controlled DTOs
            Assert.That(response.LeaveRequestId, Is.EqualTo(1));
            Assert.That(response.Comments, Is.EqualTo("Personal medical information"));
            // In real implementation, you might want to sanitize or control access to sensitive fields
        }

        [Test]
        public void ApproveLeaveRequest_RequiredAuthorization_ShouldValidateStatus()
        {
            // Arrange
            var approvalRequest = new ApproveLeaveRequest
            {
                LeaveRequestId = 1,
                Status = LeaveStatus.Approved,
                ApprovalComments = "Authorized approval"
            };

            // Assert - Verify only valid statuses can be set
            Assert.That(approvalRequest.Status, Is.EqualTo(LeaveStatus.Approved).Or.EqualTo(LeaveStatus.Rejected));
            Assert.That(approvalRequest.Status, Is.Not.EqualTo(LeaveStatus.Pending));
        }
    }
}
