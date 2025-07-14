using AutoMapper;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Features.LeaveRequest.Commands;
using LeaveManagement.Application.Interfaces.Repositories;
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
    public class CreateLeaveRequestCommandHandlerTests
    {
        private ILeaveRequestRepositoryAsync _repository;
        private IMapper _mapper;
        private CreateLeaveRequestCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repository = Substitute.For<ILeaveRequestRepositoryAsync>();
            _mapper = Substitute.For<IMapper>();
            _handler = new CreateLeaveRequestCommandHandler(_repository, _mapper);
        }

        [Test]
        public async Task Handle_ValidRequest_CreatesLeaveRequest()
        {
            // Arrange
            var request = new LeaveRequestRequest
            {
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(15),
                LeaveType = LeaveType.Annual,
                Comments = "Vacation"
            };

            var command = new CreateLeaveRequestCommand { LeaveRequest = request };
            var leaveRequestEntity = new LeaveRequest
            {
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(15),
                LeaveType = LeaveType.Annual,
                Status = LeaveStatus.Pending
            };

            var createdEntity = new LeaveRequest
            {
                LeaveRequestId = 1,
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(15),
                LeaveType = LeaveType.Annual,
                Status = LeaveStatus.Pending
            };

            var response = new LeaveRequestResponse
            {
                LeaveRequestId = 1,
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(15),
                LeaveType = LeaveType.Annual,
                Status = LeaveStatus.Pending
            };

            _mapper.Map<LeaveRequest>(request).Returns(leaveRequestEntity);
            _repository.AddAsync(Arg.Any<LeaveRequest>()).Returns(createdEntity);
            _mapper.Map<LeaveRequestResponse>(createdEntity).Returns(response);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data.LeaveRequestId, Is.EqualTo(1));
            Assert.That(result.Data.Status, Is.EqualTo(LeaveStatus.Pending));
            Assert.That(result.Message, Is.EqualTo("Leave request created successfully"));

            await _repository.Received(1).AddAsync(Arg.Any<LeaveRequest>());
        }
    }
}
