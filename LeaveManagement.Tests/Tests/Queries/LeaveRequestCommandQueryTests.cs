using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Features.LeaveRequest.Commands;
using LeaveManagement.Application.Features.LeaveRequest.Queries;
using LeaveManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Tests.Tests.Queries
{
    [TestFixture]
    public class LeaveRequestCommandQueryTests
    {
        [Test]
        public void CreateLeaveRequestCommand_Properties_ShouldBeSettable()
        {
            // Arrange
            var leaveRequest = new LeaveRequestRequest
            {
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(15),
                LeaveType = LeaveType.Annual
            };

            // Act
            var command = new CreateLeaveRequestCommand
            {
                LeaveRequest = leaveRequest
            };

            // Assert
            Assert.That(command.LeaveRequest, Is.Not.Null);
            Assert.That(command.LeaveRequest.EmployeeId, Is.EqualTo(1));
            Assert.That(command.LeaveRequest.LeaveType, Is.EqualTo(LeaveType.Annual));
        }

        [Test]
        public void ApproveLeaveRequestCommand_Properties_ShouldBeSettable()
        {
            // Arrange
            var approvalRequest = new ApproveLeaveRequest
            {
                LeaveRequestId = 1,
                Status = LeaveStatus.Approved,
                ApprovalComments = "Approved"
            };

            // Act
            var command = new ApproveLeaveRequestCommand
            {
                ApprovalRequest = approvalRequest,
                ApproverId = 2
            };

            // Assert
            Assert.That(command.ApprovalRequest, Is.Not.Null);
            Assert.That(command.ApproverId, Is.EqualTo(2));
            Assert.That(command.ApprovalRequest.Status, Is.EqualTo(LeaveStatus.Approved));
        }

        [Test]
        public void RejectLeaveRequestCommand_Properties_ShouldBeSettable()
        {
            // Act
            var command = new RejectLeaveRequestCommand
            {
                LeaveRequestId = 1,
                RejectedById = 2,
                RejectionComments = "Not enough coverage"
            };

            // Assert
            Assert.That(command.LeaveRequestId, Is.EqualTo(1));
            Assert.That(command.RejectedById, Is.EqualTo(2));
            Assert.That(command.RejectionComments, Is.EqualTo("Not enough coverage"));
        }

        [Test]
        public void GetPendingApprovalsQuery_Properties_ShouldBeSettable()
        {
            // Act
            var query = new GetPendingApprovalsQuery
            {
                ApproverId = 1
            };

            // Assert
            Assert.That(query.ApproverId, Is.EqualTo(1));
        }

        [Test]
        public void GetLeaveRequestsByEmployeeQuery_Properties_ShouldBeSettable()
        {
            // Act
            var query = new GetLeaveRequestsByEmployeeQuery
            {
                EmployeeId = 1
            };

            // Assert
            Assert.That(query.EmployeeId, Is.EqualTo(1));
        }

        [Test]
        public void GetApprovalsForManagerQuery_Properties_ShouldBeSettable()
        {
            // Act
            var query = new GetApprovalsForManagerQuery
            {
                ManagerId = 1
            };

            // Assert
            Assert.That(query.ManagerId, Is.EqualTo(1));
        }
    }
}
