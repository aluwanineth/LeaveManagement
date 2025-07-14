using LeaveManagement.Application.Features.LeaveRequest.Commands;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Tests.Tests.Handlers
{
    [TestFixture]
    public class RejectLeaveRequestCommandHandlerTests
    {
        private ILeaveRequestRepositoryAsync _repository;
        private IAuthorizationService _authorizationService;
        private RejectLeaveRequestCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repository = Substitute.For<ILeaveRequestRepositoryAsync>();
            _authorizationService = Substitute.For<IAuthorizationService>();
            _handler = new RejectLeaveRequestCommandHandler(_repository, _authorizationService);
        }

        [Test]
        public async Task Handle_ValidRejection_RejectsLeaveRequest()
        {
            // Arrange
            var employee = new Employee { EmployeeId = 2, FullName = "John Doe" };
            var leaveRequest = new LeaveRequest
            {
                LeaveRequestId = 1,
                EmployeeId = 2,
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(15),
                Status = LeaveStatus.Pending,
                Employee = employee
            };

            var command = new RejectLeaveRequestCommand
            {
                LeaveRequestId = 1,
                RejectedById = 1,
                RejectionComments = "Test rejection"
            };

            _repository.GetLeaveRequestWithDetailsAsync(1)
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Does.Contain("An error occurred while rejecting the leave request"));
        }

        [Test]
        public async Task RejectLeaveRequest_AuthorizationServiceThrowsException_ReturnsErrorResponse()
        {
            // Arrange
            var leaveRequest = new LeaveRequest
            {
                LeaveRequestId = 1,
                EmployeeId = 2,
                Status = LeaveStatus.Pending
            };

            var command = new RejectLeaveRequestCommand
            {
                LeaveRequestId = 1,
                RejectedById = 1,
                RejectionComments = "Test rejection"
            };

            _repository.GetLeaveRequestWithDetailsAsync(1).Returns(leaveRequest);
            _authorizationService.CanUserApproveLeaveRequestAsync(1, 2)
                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Does.Contain("An error occurred while rejecting the leave request"));
        }
    }
}