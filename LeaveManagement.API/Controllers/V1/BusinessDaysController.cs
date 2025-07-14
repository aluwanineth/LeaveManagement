using Asp.Versioning;
using LeaveManagement.Application.DTOs.BusinessDays;
using LeaveManagement.Application.Features.BusinessDays.Queries;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers.V1;

[ApiVersion("1.0")]
public class BusinessDaysController(IMediator mediator) :  BaseApiController(mediator)
{
    

    [HttpGet("public-holidays/{year}")]
    public async Task<ActionResult<List<PublicHolidayDto>>> GetPublicHolidays(int year)
    {
        try
        {
            var query = new GetPublicHolidaysQuery(year);
            var result = await Mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving public holidays: {ex.Message}");
        }
    }

    [HttpGet("is-business-day/{date}")]
    public async Task<ActionResult<bool>> IsBusinessDay(DateTime date)
    {
        try
        {
            var query = new IsBusinessDayQuery(date);
            var result = await Mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error checking business day: {ex.Message}");
        }
    }

    [HttpPost("validate-date-range")]
    public async Task<ActionResult<BusinessDayValidationResult>> ValidateDateRange([FromBody] DateRangeRequest request)
    {
        try
        {
            var query = new ValidateDateRangeQuery(request.StartDate, request.EndDate);
            var result = await Mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error validating date range: {ex.Message}");
        }
    }

    [HttpPost("business-days-count")]
    public async Task<ActionResult<int>> GetBusinessDaysCount([FromBody] DateRangeRequest request)
    {
        try
        {
            var query = new GetBusinessDaysCountQuery(request.StartDate, request.EndDate);
            var result = await Mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error calculating business days: {ex.Message}");
        }
    }

    [HttpPost("non-business-days")]
    public async Task<ActionResult<List<DateTime>>> GetNonBusinessDaysInRange([FromBody] DateRangeRequest request)
    {
        try
        {
            var query = new GetNonBusinessDaysQuery(request.StartDate, request.EndDate);
            var result = await Mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving non-business days: {ex.Message}");
        }
    }

    [HttpGet("next-business-day/{date}")]
    public async Task<ActionResult<DateTime>> GetNextBusinessDay(DateTime date)
    {
        try
        {
            var query = new GetNextBusinessDayQuery(date);
            var result = await Mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error finding next business day: {ex.Message}");
        }
    }

    [HttpGet("previous-business-day/{date}")]
    public async Task<ActionResult<DateTime>> GetPreviousBusinessDay(DateTime date)
    {
        try
        {
            var query = new GetPreviousBusinessDayQuery(date);
            var result = await Mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error finding previous business day: {ex.Message}");
        }
    }
}
