using AutoMapper;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Features.LeaveRequest.Queries;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Domain.Entities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Tests.Tests.Queries;

[TestFixture]
public class GetLeaveRequestsByEmployeeQueryHandlerTests
{
    private ILeaveRequestRepositoryAsync _repository;
    private IMapper _mapper;
    private GetLeaveRequestsByEmployeeQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<ILeaveRequestRepositoryAsync>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetLeaveRequestsByEmployeeQueryHandler(_repository, _mapper);
    }

    [Test]
    public async Task Handle_ValidEmployeeId_ReturnsLeaveRequests()
    {
        // Arrange
        var query = new GetLeaveRequestsByEmployeeQuery { EmployeeId = 1 };
        var leaveRequests = new List<LeaveRequest>
    {
        new LeaveRequest { LeaveRequestId = 1, EmployeeId = 1 },
        new LeaveRequest { LeaveRequestId = 2, EmployeeId = 1 }
    }.AsReadOnly();

        var responses = new List<LeaveRequestResponse>
    {
        new LeaveRequestResponse { LeaveRequestId = 1, EmployeeId = 1 },
        new LeaveRequestResponse { LeaveRequestId = 2, EmployeeId = 1 }
    }.AsReadOnly();

        _repository.GetByEmployeeIdAsync(1).Returns(leaveRequests);
        _mapper.Map<IReadOnlyList<LeaveRequestResponse>>(leaveRequests).Returns(responses);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.Succeeded, Is.True);
        Assert.That(result.Data.Count, Is.EqualTo(2));
        Assert.That(result.Message, Is.EqualTo("Leave requests retrieved successfully"));
    }
}
