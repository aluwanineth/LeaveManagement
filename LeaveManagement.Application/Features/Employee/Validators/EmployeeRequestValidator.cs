using FluentValidation;
using LeaveManagement.Application.DTOs.Employee;

namespace LeaveManagement.Application.Features.Employee.Validators;

public class EmployeeRequestValidator : AbstractValidator<EmployeeRequest>
{
    public EmployeeRequestValidator()
    {
        RuleFor(x => x.EmployeeNumber)
            .NotEmpty().WithMessage("Employee number is required")
            .Length(4, 10).WithMessage("Employee number must be between 4 and 10 characters");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.EmployeeType)
            .IsInEnum().WithMessage("Invalid employee type");
    }
}
