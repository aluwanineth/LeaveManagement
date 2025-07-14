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
    public class IsBusinessDayQueryHandlerTests
    {
        private IBusinessDayService _businessDayService;
        private IsBusinessDayQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _businessDayService = Substitute.For<IBusinessDayService>();
            _handler = new IsBusinessDayQueryHandler(_businessDayService);
        }

        [Test]
        public async Task Handle_ValidBusinessDay_ReturnsSuccessResponse()
        {
            // Arrange
            var date = new DateTime(2025, 7, 14); // Monday
            var query = new IsBusinessDayQuery(date);
            _businessDayService.IsBusinessDayAsync(date).Returns(true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.True);
            await _businessDayService.Received(1).IsBusinessDayAsync(date);
        }

        [Test]
        public async Task Handle_Weekend_ReturnsSuccessResponseWithFalse()
        {
            // Arrange
            var date = new DateTime(2025, 7, 19); // Saturday
            var query = new IsBusinessDayQuery(date);
            _businessDayService.IsBusinessDayAsync(date).Returns(false);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.False);
            await _businessDayService.Received(1).IsBusinessDayAsync(date);
        }

        [Test]
        public async Task Handle_PublicHoliday_ReturnsSuccessResponseWithFalse()
        {
            // Arrange
            var date = new DateTime(2025, 12, 25); // Christmas
            var query = new IsBusinessDayQuery(date);
            _businessDayService.IsBusinessDayAsync(date).Returns(false);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.False);
            await _businessDayService.Received(1).IsBusinessDayAsync(date);
        }
    }
}
