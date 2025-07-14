using LeaveManagement.API.Controllers.V1;
using LeaveManagement.Application.DTOs.BusinessDays;
using LeaveManagement.Application.Features.BusinessDays.Queries;
using LeaveManagement.Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace LeaveManagement.Tests.Tests.Controllers;

[TestFixture]
public class BusinessDaysControllerTests
{
    private IMediator _mediator;
    private BusinessDaysController _controller;

    [SetUp]
    public void SetUp()
    {
        _mediator = Substitute.For<IMediator>();
        _controller = new BusinessDaysController(_mediator);
    }

    [Test]
    public async Task GetPublicHolidays_ValidYear_ReturnsOkResult()
    {
        // Arrange
        var year = 2025;
        var expectedHolidays = new List<PublicHolidayDto>
        {
            new PublicHolidayDto { Date = new DateTime(2025, 1, 1), Name = "New Year's Day", IsObserved = false },
            new PublicHolidayDto { Date = new DateTime(2025, 12, 25), Name = "Christmas Day", IsObserved = false }
        };
        var expectedResponse = new Response<List<PublicHolidayDto>>(expectedHolidays);

        _mediator.Send(Arg.Any<GetPublicHolidaysQuery>()).Returns(expectedResponse);

        // Act
        var result = await _controller.GetPublicHolidays(year);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(expectedResponse));
        await _mediator.Received(1).Send(Arg.Is<GetPublicHolidaysQuery>(q => q.Year == year));
    }

    [Test]
    public async Task IsBusinessDay_ValidBusinessDay_ReturnsOkWithTrue()
    {
        // Arrange
        var date = new DateTime(2025, 7, 14); // Monday
        var expectedResponse = new Response<bool>(true);

        _mediator.Send(Arg.Any<IsBusinessDayQuery>()).Returns(expectedResponse);

        // Act
        var result = await _controller.IsBusinessDay(date);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(expectedResponse));
        await _mediator.Received(1).Send(Arg.Is<IsBusinessDayQuery>(q => q.Date == date));
    }

    [Test]
    public async Task IsBusinessDay_Weekend_ReturnsOkWithFalse()
    {
        // Arrange
        var date = new DateTime(2025, 7, 19); // Saturday
        var expectedResponse = new Response<bool>(false);

        _mediator.Send(Arg.Any<IsBusinessDayQuery>()).Returns(expectedResponse);

        // Act
        var result = await _controller.IsBusinessDay(date);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(expectedResponse));
        await _mediator.Received(1).Send(Arg.Is<IsBusinessDayQuery>(q => q.Date == date));
    }

    [Test]
    public async Task ValidateDateRange_ValidRange_ReturnsOkResult()
    {
        // Arrange
        var request = new DateRangeRequest
        {
            StartDate = new DateTime(2025, 7, 14),
            EndDate = new DateTime(2025, 7, 18)
        };

        var validationResult = new BusinessDayValidationResult
        {
            IsValid = true,
            Errors = new List<string>(),
            NonBusinessDays = new List<DateTime>(),
            BusinessDaysCount = 5
        };

        var expectedResponse = new Response<BusinessDayValidationResult>(validationResult);
        _mediator.Send(Arg.Any<ValidateDateRangeQuery>()).Returns(expectedResponse);

        // Act
        var result = await _controller.ValidateDateRange(request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(expectedResponse));
        await _mediator.Received(1).Send(Arg.Is<ValidateDateRangeQuery>(q =>
            q.StartDate == request.StartDate && q.EndDate == request.EndDate));
    }

    [Test]
    public async Task GetBusinessDaysCount_ValidRange_ReturnsOkResult()
    {
        // Arrange
        var request = new DateRangeRequest
        {
            StartDate = new DateTime(2025, 7, 14),
            EndDate = new DateTime(2025, 7, 18)
        };

        var expectedResponse = new Response<int>(5);
        _mediator.Send(Arg.Any<GetBusinessDaysCountQuery>()).Returns(expectedResponse);

        // Act
        var result = await _controller.GetBusinessDaysCount(request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(expectedResponse));
        await _mediator.Received(1).Send(Arg.Is<GetBusinessDaysCountQuery>(q =>
            q.StartDate == request.StartDate && q.EndDate == request.EndDate));
    }

    [Test]
    public async Task GetNextBusinessDay_Friday_ReturnsOkResult()
    {
        // Arrange
        var friday = new DateTime(2025, 7, 18);
        var expectedMonday = new DateTime(2025, 7, 21);
        var expectedResponse = new Response<DateTime>(expectedMonday);

        _mediator.Send(Arg.Any<GetNextBusinessDayQuery>()).Returns(expectedResponse);

        // Act
        var result = await _controller.GetNextBusinessDay(friday);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(expectedResponse));
        await _mediator.Received(1).Send(Arg.Is<GetNextBusinessDayQuery>(q => q.Date == friday));
    }

    [Test]
    public async Task GetPreviousBusinessDay_Monday_ReturnsOkResult()
    {
        // Arrange
        var monday = new DateTime(2025, 7, 21);
        var expectedFriday = new DateTime(2025, 7, 18);
        var expectedResponse = new Response<DateTime>(expectedFriday);

        _mediator.Send(Arg.Any<GetPreviousBusinessDayQuery>()).Returns(expectedResponse);

        // Act
        var result = await _controller.GetPreviousBusinessDay(monday);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(expectedResponse));
        await _mediator.Received(1).Send(Arg.Is<GetPreviousBusinessDayQuery>(q => q.Date == monday));
    }

    [Test]
    public async Task GetPublicHolidays_ThrowsException_Returns500()
    {
        // Arrange
        var year = 2025;
        _mediator.Send(Arg.Any<GetPublicHolidaysQuery>())
            .Returns(Task.FromException<Response<List<PublicHolidayDto>>>(new Exception("Database error")));

        // Act
        var result = await _controller.GetPublicHolidays(year);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task GetNonBusinessDaysInRange_ValidRange_ReturnsOkResult()
    {
        // Arrange
        var request = new DateRangeRequest
        {
            StartDate = new DateTime(2025, 7, 14),
            EndDate = new DateTime(2025, 7, 20)
        };

        var expectedNonBusinessDays = new List<DateTime>
            {
                new DateTime(2025, 7, 19), // Saturday
                new DateTime(2025, 7, 20)  // Sunday
            };

        var expectedResponse = new Response<List<DateTime>>(expectedNonBusinessDays);
        _mediator.Send(Arg.Any<GetNonBusinessDaysQuery>()).Returns(expectedResponse);

        // Act
        var result = await _controller.GetNonBusinessDaysInRange(request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(expectedResponse));
        await _mediator.Received(1).Send(Arg.Is<GetNonBusinessDaysQuery>(q =>
            q.StartDate == request.StartDate && q.EndDate == request.EndDate));
    }
}