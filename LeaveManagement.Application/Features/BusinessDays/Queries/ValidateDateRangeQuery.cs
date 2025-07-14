using LeaveManagement.Application.DTOs.BusinessDays;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.BusinessDays.Queries;

public class ValidateDateRangeQuery(DateTime startDate, DateTime endDate) : IRequest<Response<BusinessDayValidationResult>>
{
    public DateTime StartDate { get; set; } = startDate;
    public DateTime EndDate { get; set; } = endDate;
}
