using AutoMapper;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Features.LeaveRequest.Queries;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Tests.Tests.Queries;

[TestFixture]
public class GetApprovalsForManagerQueryHandlerTests
{
    private ILeaveRequestRepositoryAsync _repository;
    private IMapper _mapper;
    private GetApprovalsForManagerQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<ILeaveRequestRepositoryAsync>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetApprovalsForManagerQueryHandler(_repository, _mapper);
    }

    [Test]
    public async Task Handle_ValidManagerId_ReturnsApprovals()
    {
        // Arrange
        var query = new GetApprovalsForManagerQuery { ManagerId = 1 };
        var leaveRequests = new List<LeaveRequest>
    {
        new LeaveRequest { LeaveRequestId = 1, Status = LeaveStatus.Approved },
        new LeaveRequest { LeaveRequestId = 2, Status = LeaveStatus.Rejected }
    }.AsReadOnly();

        var responses = new List<LeaveRequestResponse>
    {
        new LeaveRequestResponse { LeaveRequestId = 1, Status = LeaveStatus.Approved },
        new LeaveRequestResponse { LeaveRequestId = 2, Status = LeaveStatus.Rejected }
    }.AsReadOnly();

        _repository.GetLeaveRequestsForApprovalAsync(1).Returns(leaveRequests);
        _mapper.Map<IReadOnlyList<LeaveRequestResponse>>(leaveRequests).Returns(responses);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.Succeeded, Is.True);
        Assert.That(result.Data.Count, Is.EqualTo(2));
        Assert.That(result.Message, Is.EqualTo("Approval history retrieved successfully"));
    }
}
