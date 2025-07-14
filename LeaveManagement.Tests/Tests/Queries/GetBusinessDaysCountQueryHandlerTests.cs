using LeaveManagement.Application.Features.BusinessDays.Queries;
using LeaveManagement.Application.Interfaces.Services;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Tests.Tests.Queries
{
    [TestFixture]
    public class GetBusinessDaysCountQueryHandlerTests
    {
        private IBusinessDayService _businessDayService;
        private GetBusinessDaysCountQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _businessDayService = Substitute.For<IBusinessDayService>();
            _handler = new GetBusinessDaysCountQueryHandler(_businessDayService);
        }

        [Test]
        public async Task Handle_ValidDateRange_ReturnsCorrectCount()
        {
            // Arrange
            var startDate = new DateTime(2025, 7, 14); // Monday
            var endDate = new DateTime(2025, 7, 18);   // Friday
            var query = new GetBusinessDaysCountQuery(startDate, endDate);
            var expectedCount = 5;

            _businessDayService.GetBusinessDaysCountAsync(startDate, endDate).Returns(expectedCount);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.EqualTo(expectedCount));
            Assert.That(result.Message, Is.EqualTo("Business days count retrieved successfully"));
            await _businessDayService.Received(1).GetBusinessDaysCountAsync(startDate, endDate);
        }

        [Test]
        public async Task Handle_SingleDay_ReturnsOne()
        {
            // Arrange
            var startDate = new DateTime(2025, 7, 14); // Monday
            var endDate = new DateTime(2025, 7, 14);   // Same Monday
            var query = new GetBusinessDaysCountQuery(startDate, endDate);
            var expectedCount = 1;

            _businessDayService.GetBusinessDaysCountAsync(startDate, endDate).Returns(expectedCount);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.EqualTo(expectedCount));
            await _businessDayService.Received(1).GetBusinessDaysCountAsync(startDate, endDate);
        }

        [Test]
        public async Task Handle_WeekendOnly_ReturnsZero()
        {
            // Arrange
            var startDate = new DateTime(2025, 7, 19); // Saturday
            var endDate = new DateTime(2025, 7, 20);   // Sunday
            var query = new GetBusinessDaysCountQuery(startDate, endDate);
            var expectedCount = 0;

            _businessDayService.GetBusinessDaysCountAsync(startDate, endDate).Returns(expectedCount);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.EqualTo(expectedCount));
            await _businessDayService.Received(1).GetBusinessDaysCountAsync(startDate, endDate);
        }
    }
}
