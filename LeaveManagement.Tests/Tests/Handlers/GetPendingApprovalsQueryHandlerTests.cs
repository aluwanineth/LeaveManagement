using AutoMapper;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Features.LeaveRequest.Queries;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using NSubstitute;

namespace LeaveManagement.Tests.Tests.Handlers;

[TestFixture]
public class GetPendingApprovalsQueryHandlerTests
{
    private ILeaveRequestRepositoryAsync _leaveRequestRepository;
    private IMapper _mapper;
    private GetPendingApprovalsQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _leaveRequestRepository = Substitute.For<ILeaveRequestRepositoryAsync>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetPendingApprovalsQueryHandler(_leaveRequestRepository, _mapper);
    }

    [Test]
    public async Task Handle_NoApprovals_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetPendingApprovalsQuery { ApproverId = 1 };
        var emptyList = new List<LeaveRequest>().AsReadOnly();
        var emptyResponseList = new List<LeaveRequestResponse>().AsReadOnly();

        _leaveRequestRepository.GetPendingRequestsForManagerAsync(1).Returns(emptyList);
        _mapper.Map<IReadOnlyList<LeaveRequestResponse>>(emptyList).Returns(emptyResponseList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.Succeeded, Is.True);
        Assert.That(result.Data.Count, Is.EqualTo(0));
    }
}
