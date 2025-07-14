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
    public class GetNextBusinessDayQueryHandlerTests
    {
        private IBusinessDayService _businessDayService;
        private GetNextBusinessDayQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _businessDayService = Substitute.For<IBusinessDayService>();
            _handler = new GetNextBusinessDayQueryHandler(_businessDayService);
        }

        [Test]
        public async Task Handle_Friday_ReturnsNextMonday()
        {
            // Arrange
            var friday = new DateTime(2025, 7, 18);
            var expectedMonday = new DateTime(2025, 7, 21);
            var query = new GetNextBusinessDayQuery(friday);

            _businessDayService.GetNextBusinessDayAsync(friday).Returns(expectedMonday);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.EqualTo(expectedMonday));
            Assert.That(result.Message, Is.EqualTo("Next business day retrieved successfully"));
            await _businessDayService.Received(1).GetNextBusinessDayAsync(friday);
        }

        [Test]
        public async Task Handle_Thursday_ReturnsNextFriday()
        {
            // Arrange
            var thursday = new DateTime(2025, 7, 17);
            var expectedFriday = new DateTime(2025, 7, 18);
            var query = new GetNextBusinessDayQuery(thursday);

            _businessDayService.GetNextBusinessDayAsync(thursday).Returns(expectedFriday);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.EqualTo(expectedFriday));
            await _businessDayService.Received(1).GetNextBusinessDayAsync(thursday);
        }

        [Test]
        public async Task Handle_Saturday_ReturnsNextMonday()
        {
            // Arrange
            var saturday = new DateTime(2025, 7, 19);
            var expectedMonday = new DateTime(2025, 7, 21);
            var query = new GetNextBusinessDayQuery(saturday);

            _businessDayService.GetNextBusinessDayAsync(saturday).Returns(expectedMonday);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.EqualTo(expectedMonday));
            await _businessDayService.Received(1).GetNextBusinessDayAsync(saturday);
        }
    }
}
