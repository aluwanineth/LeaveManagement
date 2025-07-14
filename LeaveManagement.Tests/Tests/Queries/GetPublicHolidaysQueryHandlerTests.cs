using LeaveManagement.Application.Features.BusinessDays.Queries;
using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Application.Services;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Tests.Tests.Queries
{
    [TestFixture]
    public class GetPublicHolidaysQueryHandlerTests
    {
        private IBusinessDayService _businessDayService;
        private GetPublicHolidaysQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _businessDayService = Substitute.For<IBusinessDayService>();
            _handler = new GetPublicHolidaysQueryHandler(_businessDayService);
        }

        [Test]
        public async Task Handle_ValidYear_ReturnsSuccessResponse()
        {
            // Arrange
            var year = 2025;
            var query = new GetPublicHolidaysQuery(year);
            var expectedHolidays = new List<PublicHolidayInfo>
        {
            new PublicHolidayInfo(new DateTime(2025, 1, 1), "New Year's Day"),
            new PublicHolidayInfo(new DateTime(2025, 12, 25), "Christmas Day")
        };

            _businessDayService.GetSouthAfricanPublicHolidays(year).Returns(expectedHolidays);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count, Is.EqualTo(2));
            Assert.That(result.Data[0].Name, Is.EqualTo("New Year's Day"));
            Assert.That(result.Data[1].Name, Is.EqualTo("Christmas Day"));
            Assert.That(result.Data[0].IsObserved, Is.False);
            Assert.That(result.Data[1].IsObserved, Is.False);

            _businessDayService.Received(1).GetSouthAfricanPublicHolidays(year);
        }

        [Test]
        public async Task Handle_YearWithObservedHolidays_ReturnsCorrectlyMappedData()
        {
            // Arrange
            var year = 2025;
            var query = new GetPublicHolidaysQuery(year);
            var expectedHolidays = new List<PublicHolidayInfo>
        {
            new PublicHolidayInfo(new DateTime(2025, 4, 27), "Freedom Day"),
            new PublicHolidayInfo(new DateTime(2025, 4, 28), "Public holiday Freedom Day observed")
        };

            _businessDayService.GetSouthAfricanPublicHolidays(year).Returns(expectedHolidays);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data.Count, Is.EqualTo(2));
            Assert.That(result.Data[0].IsObserved, Is.False); // Original holiday
            Assert.That(result.Data[1].IsObserved, Is.True);  // Observed holiday
        }
    }
}
