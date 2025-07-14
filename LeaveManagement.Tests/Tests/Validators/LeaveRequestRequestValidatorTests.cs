using FluentValidation.TestHelper;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Features.LeaveRequest.Validators;
using LeaveManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Tests.Tests.Validators
{
    [TestFixture]
    public class LeaveRequestRequestValidatorTests
    {
        private LeaveRequestRequestValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new LeaveRequestRequestValidator();
        }

        [Test]
        public void Validate_ValidLeaveRequest_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var request = new LeaveRequestRequest
            {
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(5),
                LeaveType = LeaveType.Annual,
                Comments = "Family vacation"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void Validate_InvalidEmployeeId_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LeaveRequestRequest
            {
                EmployeeId = 0,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(5),
                LeaveType = LeaveType.Annual
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EmployeeId)
                .WithErrorMessage("Employee ID must be greater than 0");
        }

        [Test]
        public void Validate_StartDateInPast_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LeaveRequestRequest
            {
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(-1),
                EndDate = DateTime.Today.AddDays(5),
                LeaveType = LeaveType.Annual
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.StartDate)
                .WithErrorMessage("Start date cannot be in the past");
        }

        [Test]
        public void Validate_EndDateBeforeStartDate_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LeaveRequestRequest
            {
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(5),
                EndDate = DateTime.Today.AddDays(3),
                LeaveType = LeaveType.Annual
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EndDate)
                .WithErrorMessage("End date must be after start date");
        }

        [Test]
        public void Validate_CommentsTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var longComments = new string('A', 501);
            var request = new LeaveRequestRequest
            {
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(5),
                LeaveType = LeaveType.Annual,
                Comments = longComments
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Comments)
                .WithErrorMessage("Comments cannot exceed 500 characters");
        }
    }
}
