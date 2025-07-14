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
    public class GetNonBusinessDaysQueryHandlerTests
    {
        private IBusinessDayService _businessDayService;
        private GetNonBusinessDaysQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _businessDayService = Substitute.For<IBusinessDayService>();
            _handler = new GetNonBusinessDaysQueryHandler(_businessDayService);
        }

        [Test]
        public async Task Handle_ValidDateRange_ReturnsNonBusinessDays()
        {
            // Arrange
            var startDate = new DateTime(2025, 7, 14); // Monday
            var endDate = new DateTime(2025, 7, 20);   // Sunday
            var query = new GetNonBusinessDaysQuery(startDate, endDate);
            var expectedNonBusinessDays = new List<DateTime>
            {
                new DateTime(2025, 7, 19), // Saturday
                new DateTime(2025, 7, 20)  // Sunday
            };

            _businessDayService.GetNonBusinessDaysInRangeAsync(startDate, endDate)
                .Returns(expectedNonBusinessDays);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count, Is.EqualTo(2));
            Assert.That(result.Data, Contains.Item(new DateTime(2025, 7, 19)));
            Assert.That(result.Data, Contains.Item(new DateTime(2025, 7, 20)));
            Assert.That(result.Message, Is.EqualTo("Non-business days retrieved successfully"));
            await _businessDayService.Received(1).GetNonBusinessDaysInRangeAsync(startDate, endDate);
        }

        [Test]
        public async Task Handle_OnlyBusinessDays_ReturnsEmptyList()
        {
            // Arrange
            var startDate = new DateTime(2025, 7, 14); // Monday
            var endDate = new DateTime(2025, 7, 18);   // Friday
            var query = new GetNonBusinessDaysQuery(startDate, endDate);
            var expectedNonBusinessDays = new List<DateTime>();

            _businessDayService.GetNonBusinessDaysInRangeAsync(startDate, endDate)
                .Returns(expectedNonBusinessDays);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count, Is.EqualTo(0));
            await _businessDayService.Received(1).GetNonBusinessDaysInRangeAsync(startDate, endDate);
        }

        [Test]
        public async Task Handle_RangeWithHolidays_ReturnsWeekendsAndHolidays()
        {
            // Arrange
            var startDate = new DateTime(2025, 12, 22); // Monday
            var endDate = new DateTime(2025, 12, 28);   // Sunday
            var query = new GetNonBusinessDaysQuery(startDate, endDate);
            var expectedNonBusinessDays = new List<DateTime>
            {
                new DateTime(2025, 12, 25), // Christmas - Thursday
                new DateTime(2025, 12, 26), // Day of Goodwill - Friday
                new DateTime(2025, 12, 27), // Saturday
                new DateTime(2025, 12, 28)  // Sunday
            };

            _businessDayService.GetNonBusinessDaysInRangeAsync(startDate, endDate)
                .Returns(expectedNonBusinessDays);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data.Count, Is.EqualTo(4));
            await _businessDayService.Received(1).GetNonBusinessDaysInRangeAsync(startDate, endDate);
        }
    }
}