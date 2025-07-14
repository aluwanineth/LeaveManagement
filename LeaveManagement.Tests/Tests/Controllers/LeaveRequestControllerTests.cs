using LeaveManagement.API.Controllers.V1;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Features.LeaveRequest.Commands;
using LeaveManagement.Application.Features.LeaveRequest.Queries;
using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Application.Wrappers;
using LeaveManagement.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace LeaveManagement.Tests.Tests.Controllers;

[TestFixture]
public class LeaveRequestControllerTests
{
    private IMediator _mediator;
    private IAuthenticatedUserService _authenticatedUserService;
    private LeaveRequestController _controller;

    [SetUp]
    public void SetUp()
    {
        _mediator = Substitute.For<IMediator>();
        _authenticatedUserService = Substitute.For<IAuthenticatedUserService>();
        _controller = new LeaveRequestController(_mediator, _authenticatedUserService);
    }

    [Test]
    public async Task CreateLeaveRequest_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var leaveRequestRequest = new LeaveRequestRequest
        {
            EmployeeId = 1,
            StartDate = DateTime.Today.AddDays(10),
            EndDate = DateTime.Today.AddDays(15),
            LeaveType = LeaveType.Annual,
            Comments = "Vacation"
        };

        var expectedResponse = new Response<LeaveRequestResponse>(new LeaveRequestResponse());
        _mediator.Send(Arg.Any<CreateLeaveRequestCommand>()).Returns(expectedResponse);

        // Act
        var result = await _controller.CreateLeaveRequest(leaveRequestRequest);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(expectedResponse));

        await _mediator.Received(1).Send(Arg.Any<CreateLeaveRequestCommand>());
    }

    [Test]
    public async Task GetPendingApprovals_UserHasEmployeeId_ReturnsOkResult()
    {
        // Arrange
        _authenticatedUserService.EmployeeId.Returns(1);
        var expectedResponse = new Response<IReadOnlyList<LeaveRequestResponse>>(
            new List<LeaveRequestResponse>().AsReadOnly());
        _mediator.Send(Arg.Any<GetPendingApprovalsQuery>()).Returns(expectedResponse);

        // Act
        var result = await _controller.GetPendingApprovals();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        await _mediator.Received(1).Send(Arg.Any<GetPendingApprovalsQuery>());
    }

    [Test]
    public async Task GetPendingApprovals_UserHasNoEmployeeId_ReturnsBadRequest()
    {
        // Arrange
        _authenticatedUserService.EmployeeId.Returns((int?)null);

        // Act
        var result = await _controller.GetPendingApprovals();

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        var response = badRequestResult.Value as Response<string>;
        Assert.That(response.Message, Is.EqualTo("Employee ID not found"));
    }

    [Test]
    public async Task ApproveLeaveRequest_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        _authenticatedUserService.EmployeeId.Returns(1);
        var approveRequest = new ApproveLeaveRequest
        {
            LeaveRequestId = 1,
            Status = LeaveStatus.Approved,
            ApprovalComments = "Approved"
        };

        var expectedResponse = new Response<LeaveRequestResponse>(new LeaveRequestResponse());
        _mediator.Send(Arg.Any<ApproveLeaveRequestCommand>()).Returns(expectedResponse);

        // Act
        var result = await _controller.ApproveLeaveRequest(approveRequest);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        await _mediator.Received(1).Send(Arg.Any<ApproveLeaveRequestCommand>());
    }
}
