using FluentValidation;
using LeaveManagement.Application.DTOs.LeaveRequest;

namespace LeaveManagement.Application.Features.LeaveRequest.Validators;

public class LeaveRequestRequestValidator : AbstractValidator<LeaveRequestRequest>
{
    public LeaveRequestRequestValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("Employee ID must be greater than 0");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the past");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be after start date");

        RuleFor(x => x.LeaveType)
            .IsInEnum().WithMessage("Invalid leave type");

        RuleFor(x => x.Comments)
            .MaximumLength(500).WithMessage("Comments cannot exceed 500 characters");
    }
}
