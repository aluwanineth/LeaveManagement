using FluentValidation.TestHelper;
using LeaveManagement.Application.DTOs.Employee;
using LeaveManagement.Application.Features.Employee.Validators;
using LeaveManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Tests.Tests.Validators
{
    [TestFixture]
    public class EmployeeRequestValidatorTests
    {
        private EmployeeRequestValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new EmployeeRequestValidator();
        }

        [Test]
        public void Validate_ValidEmployeeRequest_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var request = new EmployeeRequest
            {
                EmployeeNumber = "EMP001",
                FullName = "John Doe",
                Email = "john.doe@acme.com",
                EmployeeType = EmployeeType.Employee
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void Validate_EmptyEmployeeNumber_ShouldHaveValidationError()
        {
            // Arrange
            var request = new EmployeeRequest
            {
                EmployeeNumber = "",
                FullName = "John Doe",
                Email = "john.doe@acme.com",
                EmployeeType = EmployeeType.Employee
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EmployeeNumber)
                .WithErrorMessage("Employee number is required");
        }

        [Test]
        public void Validate_InvalidEmail_ShouldHaveValidationError()
        {
            // Arrange
            var request = new EmployeeRequest
            {
                EmployeeNumber = "EMP001",
                FullName = "John Doe",
                Email = "invalid-email",
                EmployeeType = EmployeeType.Employee
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("Invalid email format");
        }

        [Test]
        public void Validate_EmployeeNumberTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var request = new EmployeeRequest
            {
                EmployeeNumber = "VERYLONGEMPLOYEENUMBER123",
                FullName = "John Doe",
                Email = "john.doe@acme.com",
                EmployeeType = EmployeeType.Employee
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EmployeeNumber)
                .WithErrorMessage("Employee number must be between 4 and 10 characters");
        }
    }
}
