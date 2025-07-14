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
    public class GetPreviousBusinessDayQueryHandlerTests
    {
        private IBusinessDayService _businessDayService;
        private GetPreviousBusinessDayQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _businessDayService = Substitute.For<IBusinessDayService>();
            _handler = new GetPreviousBusinessDayQueryHandler(_businessDayService);
        }

        [Test]
        public async Task Handle_Monday_ReturnsPreviousFriday()
        {
            // Arrange
            var monday = new DateTime(2025, 7, 21);
            var expectedFriday = new DateTime(2025, 7, 18);
            var query = new GetPreviousBusinessDayQuery(monday);

            _businessDayService.GetPreviousBusinessDayAsync(monday).Returns(expectedFriday);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.EqualTo(expectedFriday));
            Assert.That(result.Message, Is.EqualTo("Previous business day retrieved successfully"));
            await _businessDayService.Received(1).GetPreviousBusinessDayAsync(monday);
        }

        [Test]
        public async Task Handle_Tuesday_ReturnsPreviousMonday()
        {
            // Arrange
            var tuesday = new DateTime(2025, 7, 15);
            var expectedMonday = new DateTime(2025, 7, 14);
            var query = new GetPreviousBusinessDayQuery(tuesday);

            _businessDayService.GetPreviousBusinessDayAsync(tuesday).Returns(expectedMonday);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.EqualTo(expectedMonday));
            await _businessDayService.Received(1).GetPreviousBusinessDayAsync(tuesday);
        }

        [Test]
        public async Task Handle_Sunday_ReturnsPreviousFriday()
        {
            // Arrange
            var sunday = new DateTime(2025, 7, 20);
            var expectedFriday = new DateTime(2025, 7, 18);
            var query = new GetPreviousBusinessDayQuery(sunday);

            _businessDayService.GetPreviousBusinessDayAsync(sunday).Returns(expectedFriday);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.EqualTo(expectedFriday));
            await _businessDayService.Received(1).GetPreviousBusinessDayAsync(sunday);
        }
    }
}