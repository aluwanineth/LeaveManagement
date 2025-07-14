using AutoMapper;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Features.LeaveRequest.Commands;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Tests.Tests.Handlers
{
    [TestFixture]
    public class ApproveLeaveRequestCommandHandlerTests
    {
        private ILeaveRequestRepositoryAsync _repository;
        private IAuthorizationService _authorizationService;
        private IMapper _mapper;
        private ApproveLeaveRequestCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repository = Substitute.For<ILeaveRequestRepositoryAsync>();
            _authorizationService = Substitute.For<IAuthorizationService>();
            _mapper = Substitute.For<IMapper>();
            _handler = new ApproveLeaveRequestCommandHandler(_repository, _authorizationService, _mapper);
        }

        [Test]
        public async Task Handle_ValidApproval_UpdatesLeaveRequest()
        {
            // Arrange
            var leaveRequest = new LeaveRequest
            {
                LeaveRequestId = 1,
                EmployeeId = 2,
                Status = LeaveStatus.Pending
            };

            var approvalRequest = new ApproveLeaveRequest
            {
                LeaveRequestId = 1,
                Status = LeaveStatus.Approved,
                ApprovalComments = "Approved"
            };

            var command = new ApproveLeaveRequestCommand
            {
                ApprovalRequest = approvalRequest,
                ApproverId = 1
            };

            var response = new LeaveRequestResponse
            {
                LeaveRequestId = 1,
                Status = LeaveStatus.Approved
            };

            _repository.GetByIdAsync(1).Returns(leaveRequest);
            _authorizationService.CanUserApproveLeaveRequestAsync(1, 2).Returns(true);
            _mapper.Map<LeaveRequestResponse>(leaveRequest).Returns(response);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Message, Is.EqualTo("Leave request processed successfully"));
            Assert.That(leaveRequest.Status, Is.EqualTo(LeaveStatus.Approved));
            Assert.That(leaveRequest.ApprovedById, Is.EqualTo(1));

            await _repository.Received(1).UpdateAsync(leaveRequest);
        }

        [Test]
        public async Task Handle_LeaveRequestNotFound_ReturnsError()
        {
            // Arrange
            var command = new ApproveLeaveRequestCommand
            {
                ApprovalRequest = new ApproveLeaveRequest { LeaveRequestId = 999 },
                ApproverId = 1
            };

            _repository.GetByIdAsync(999).Returns((LeaveRequest)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Leave request not found"));
        }

        [Test]
        public async Task Handle_UnauthorizedUser_ReturnsError()
        {
            // Arrange
            var leaveRequest = new LeaveRequest
            {
                LeaveRequestId = 1,
                EmployeeId = 2,
                Status = LeaveStatus.Pending
            };

            var command = new ApproveLeaveRequestCommand
            {
                ApprovalRequest = new ApproveLeaveRequest { LeaveRequestId = 1 },
                ApproverId = 3
            };

            _repository.GetByIdAsync(1).Returns(leaveRequest);
            _authorizationService.CanUserApproveLeaveRequestAsync(3, 2).Returns(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("You are not authorized to approve this leave request"));
        }
    }

}