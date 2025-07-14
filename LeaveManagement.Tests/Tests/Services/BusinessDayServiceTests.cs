using FluentAssertions.Common;
using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Tests.Tests.Services
{
    [TestFixture]
    public class BusinessDayServiceTests
    {
        private IBusinessDayService _businessDayService;

        [SetUp]
        public void SetUp()
        {
            _businessDayService = new BusinessDayService();
        }

        [TestCase("2025-07-14", true, Description = "Monday should be a business day")]
        [TestCase("2025-07-15", true, Description = "Tuesday should be a business day")]
        [TestCase("2025-07-16", true, Description = "Wednesday should be a business day")]
        [TestCase("2025-07-17", true, Description = "Thursday should be a business day")]
        [TestCase("2025-07-18", true, Description = "Friday should be a business day")]
        [TestCase("2025-07-19", false, Description = "Saturday should not be a business day")]
        [TestCase("2025-07-20", false, Description = "Sunday should not be a business day")]
        public async Task IsBusinessDayAsync_WeekDays_ReturnsExpectedResult(string dateString, bool expected)
        {
            // Arrange
            var date = DateTime.Parse(dateString);

            // Act
            var result = await _businessDayService.IsBusinessDayAsync(date);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("2025-01-01", false, Description = "New Year's Day should not be a business day")]
        [TestCase("2025-03-21", false, Description = "Human Rights Day should not be a business day")]
        [TestCase("2025-04-27", false, Description = "Freedom Day should not be a business day")]
        [TestCase("2025-05-01", false, Description = "Workers' Day should not be a business day")]
        [TestCase("2025-06-16", false, Description = "Youth Day should not be a business day")]
        [TestCase("2025-08-09", false, Description = "National Women's Day should not be a business day")]
        [TestCase("2025-09-24", false, Description = "Heritage Day should not be a business day")]
        [TestCase("2025-12-16", false, Description = "Day of Reconciliation should not be a business day")]
        [TestCase("2025-12-25", false, Description = "Christmas Day should not be a business day")]
        [TestCase("2025-12-26", false, Description = "Day of Goodwill should not be a business day")]
        public async Task IsBusinessDayAsync_SouthAfricanPublicHolidays_ReturnsFalse(string dateString, bool expected)
        {
            // Arrange
            var date = DateTime.Parse(dateString);

            // Act
            var result = await _businessDayService.IsBusinessDayAsync(date);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("2025-04-18", false, Description = "Good Friday 2025 should not be a business day")]
        [TestCase("2025-04-21", false, Description = "Family Day 2025 should not be a business day")]
        [TestCase("2026-04-03", false, Description = "Good Friday 2026 should not be a business day")]
        [TestCase("2026-04-06", false, Description = "Family Day 2026 should not be a business day")]
        public async Task IsBusinessDayAsync_EasterBasedHolidays_ReturnsFalse(string dateString, bool expected)
        {
            // Arrange
            var date = DateTime.Parse(dateString);

            // Act
            var result = await _businessDayService.IsBusinessDayAsync(date);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetBusinessDaysCountAsync_SingleBusinessDay_ReturnsOne()
        {
            // Arrange
            var startDate = new DateTime(2025, 7, 14); // Monday
            var endDate = new DateTime(2025, 7, 14); // Same Monday

            // Act
            var result = await _businessDayService.GetBusinessDaysCountAsync(startDate, endDate);

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public async Task GetBusinessDaysCountAsync_FullWorkWeek_ReturnsFive()
        {
            // Arrange
            var startDate = new DateTime(2025, 7, 14); // Monday
            var endDate = new DateTime(2025, 7, 18); // Friday

            // Act
            var result = await _businessDayService.GetBusinessDaysCountAsync(startDate, endDate);

            // Assert
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public async Task GetBusinessDaysCountAsync_IncludingWeekend_ReturnsFive()
        {
            // Arrange
            var startDate = new DateTime(2025, 7, 14); // Monday
            var endDate = new DateTime(2025, 7, 20); // Sunday

            // Act
            var result = await _businessDayService.GetBusinessDaysCountAsync(startDate, endDate);

            // Assert
            Assert.That(result, Is.EqualTo(5)); // Mon, Tue, Wed, Thu, Fri
        }

        [Test]
        public async Task GetBusinessDaysCountAsync_OnlyWeekends_ReturnsZero()
        {
            // Arrange
            var startDate = new DateTime(2025, 7, 19); // Saturday
            var endDate = new DateTime(2025, 7, 20); // Sunday

            // Act
            var result = await _businessDayService.GetBusinessDaysCountAsync(startDate, endDate);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetBusinessDaysCountAsync_IncludingPublicHoliday_ExcludesHoliday()
        {
            // Arrange - Range including Christmas Day
            var startDate = new DateTime(2025, 12, 24); // Wednesday
            var endDate = new DateTime(2025, 12, 26); // Friday (Day of Goodwill)

            // Act
            var result = await _businessDayService.GetBusinessDaysCountAsync(startDate, endDate);

            // Assert
            Assert.That(result, Is.EqualTo(1)); // Only Dec 24th is a business day
        }

        [Test]
        public void GetBusinessDaysCountAsync_StartDateAfterEndDate_ThrowsArgumentException()
        {
            // Arrange
            var startDate = new DateTime(2025, 7, 18); // Friday
            var endDate = new DateTime(2025, 7, 14); // Monday

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _businessDayService.GetBusinessDaysCountAsync(startDate, endDate));
        }

        [Test]
        public async Task GetNextBusinessDayAsync_FromFriday_ReturnsMonday()
        {
            // Arrange
            var friday = new DateTime(2025, 7, 18);

            // Act
            var result = await _businessDayService.GetNextBusinessDayAsync(friday);

            // Assert
            var expectedMonday = new DateTime(2025, 7, 21);
            Assert.That(result, Is.EqualTo(expectedMonday));
        }

        [Test]
        public async Task GetNextBusinessDayAsync_FromThursday_ReturnsFriday()
        {
            // Arrange
            var thursday = new DateTime(2025, 7, 17);

            // Act
            var result = await _businessDayService.GetNextBusinessDayAsync(thursday);

            // Assert
            var expectedFriday = new DateTime(2025, 7, 18);
            Assert.That(result, Is.EqualTo(expectedFriday));
        }

        [Test]
        public async Task GetPreviousBusinessDayAsync_FromMonday_ReturnsFriday()
        {
            // Arrange
            var monday = new DateTime(2025, 7, 21);

            // Act
            var result = await _businessDayService.GetPreviousBusinessDayAsync(monday);

            // Assert
            var expectedFriday = new DateTime(2025, 7, 18);
            Assert.That(result, Is.EqualTo(expectedFriday));
        }

        [Test]
        public async Task GetNonBusinessDaysInRangeAsync_FullWeek_ReturnsWeekends()
        {
            // Arrange
            var startDate = new DateTime(2025, 7, 14); // Monday
            var endDate = new DateTime(2025, 7, 20); // Sunday

            // Act
            var result = await _businessDayService.GetNonBusinessDaysInRangeAsync(startDate, endDate);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result, Contains.Item(new DateTime(2025, 7, 19))); // Saturday
            Assert.That(result, Contains.Item(new DateTime(2025, 7, 20))); // Sunday
        }

        [Test]
        public async Task GetNonBusinessDaysInRangeAsync_OnlyBusinessDays_ReturnsEmpty()
        {
            // Arrange
            var startDate = new DateTime(2025, 7, 14); // Monday
            var endDate = new DateTime(2025, 7, 18); // Friday

            // Act
            var result = await _businessDayService.GetNonBusinessDaysInRangeAsync(startDate, endDate);

            // Assert
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task IsDateRangeValidForLeaveAsync_ValidBusinessDays_ReturnsTrue()
        {
            // Arrange
            var startDate = new DateTime(2025, 7, 14); // Monday
            var endDate = new DateTime(2025, 7, 18); // Friday

            // Act
            var result = await _businessDayService.IsDateRangeValidForLeaveAsync(startDate, endDate);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsDateRangeValidForLeaveAsync_StartDateIsWeekend_ReturnsFalse()
        {
            // Arrange
            var startDate = new DateTime(2025, 7, 19); // Saturday
            var endDate = new DateTime(2025, 7, 21); // Monday

            // Act
            var result = await _businessDayService.IsDateRangeValidForLeaveAsync(startDate, endDate);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetSouthAfricanPublicHolidays_2025_ReturnsCorrectFixedHolidays()
        {
            // Act
            var result = _businessDayService.GetSouthAfricanPublicHolidays(2025);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(12)); // At least 12 holidays

            // Check fixed holidays
            Assert.That(result.Any(h => h.Date.Date == new DateTime(2025, 1, 1) && h.Name == "New Year's Day"), Is.True);
            Assert.That(result.Any(h => h.Date.Date == new DateTime(2025, 3, 21) && h.Name == "Human Rights Day"), Is.True);
            Assert.That(result.Any(h => h.Date.Date == new DateTime(2025, 4, 27) && h.Name == "Freedom Day"), Is.True);
            Assert.That(result.Any(h => h.Date.Date == new DateTime(2025, 12, 25) && h.Name == "Christmas Day"), Is.True);
        }

        [Test]
        public void GetSouthAfricanPublicHolidays_2025_ReturnsCorrectEasterHolidays()
        {
            // Act
            var result = _businessDayService.GetSouthAfricanPublicHolidays(2025);

            // Assert
            // Good Friday 2025 = April 18
            Assert.That(result.Any(h => h.Date.Date == new DateTime(2025, 4, 18) && h.Name == "Good Friday"), Is.True);
            // Family Day 2025 = April 21
            Assert.That(result.Any(h => h.Date.Date == new DateTime(2025, 4, 21) && h.Name == "Family Day"), Is.True);
        }

        [Test]
        public void GetSouthAfricanPublicHolidays_ReturnsHolidaysInChronologicalOrder()
        {
            // Act
            var result = _businessDayService.GetSouthAfricanPublicHolidays(2025);

            // Assert
            var sortedResult = result.OrderBy(h => h.Date).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                Assert.That(result[i].Date, Is.EqualTo(sortedResult[i].Date));
            }
        }

        [Test]
        public async Task IsBusinessDayAsync_Monday_ReturnsTrue()
        {
            // Arrange
            var monday = new DateTime(2025, 7, 14);

            // Act
            var result = await _businessDayService.IsBusinessDayAsync(monday);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsBusinessDayAsync_Saturday_ReturnsFalse()
        {
            // Arrange
            var saturday = new DateTime(2025, 7, 19);

            // Act
            var result = await _businessDayService.IsBusinessDayAsync(saturday);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task IsBusinessDayAsync_Sunday_ReturnsFalse()
        {
            // Arrange
            var sunday = new DateTime(2025, 7, 20);

            // Act
            var result = await _businessDayService.IsBusinessDayAsync(sunday);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task IsBusinessDayAsync_ChristmasDay_ReturnsFalse()
        {
            // Arrange
            var christmas = new DateTime(2025, 12, 25);

            // Act
            var result = await _businessDayService.IsBusinessDayAsync(christmas);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetNextBusinessDayAsync_Friday_ReturnsMonday()
        {
            // Arrange
            var friday = new DateTime(2025, 7, 18);
            var expectedMonday = new DateTime(2025, 7, 21);

            // Act
            var result = await _businessDayService.GetNextBusinessDayAsync(friday);

            // Assert
            Assert.That(result, Is.EqualTo(expectedMonday));
        }

        [Test]
        public async Task GetPreviousBusinessDayAsync_Monday_ReturnsFriday()
        {
            // Arrange
            var monday = new DateTime(2025, 7, 21);
            var expectedFriday = new DateTime(2025, 7, 18);

            // Act
            var result = await _businessDayService.GetPreviousBusinessDayAsync(monday);

            // Assert
            Assert.That(result, Is.EqualTo(expectedFriday));
        }

        [Test]
        public async Task GetBusinessDaysCountAsync_MondayToFriday_ReturnsFive()
        {
            // Arrange
            var monday = new DateTime(2025, 7, 14);
            var friday = new DateTime(2025, 7, 18);

            // Act
            var result = await _businessDayService.GetBusinessDaysCountAsync(monday, friday);

            // Assert
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void GetBusinessDaysCountAsync_StartDateAfterEndDate_ThrowsException()
        {
            // Arrange
            var startDate = new DateTime(2025, 7, 18);
            var endDate = new DateTime(2025, 7, 14);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _businessDayService.GetBusinessDaysCountAsync(startDate, endDate));
        }

        [Test]
        public async Task GetNonBusinessDaysInRangeAsync_MondayToSunday_ReturnsWeekend()
        {
            // Arrange
            var monday = new DateTime(2025, 7, 14);
            var sunday = new DateTime(2025, 7, 20);

            // Act
            var result = await _businessDayService.GetNonBusinessDaysInRangeAsync(monday, sunday);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result, Contains.Item(new DateTime(2025, 7, 19))); // Saturday
            Assert.That(result, Contains.Item(new DateTime(2025, 7, 20))); // Sunday
        }

        [Test]
        public void GetSouthAfricanPublicHolidays_2025_ReturnsCorrectHolidays()
        {
            // Act
            var holidays = _businessDayService.GetSouthAfricanPublicHolidays(2025);

            // Assert
            Assert.That(holidays.Count, Is.GreaterThan(10));
            Assert.That(holidays.Any(h => h.Name == "New Year's Day"), Is.True);
            Assert.That(holidays.Any(h => h.Name == "Christmas Day"), Is.True);
            Assert.That(holidays.Any(h => h.Name == "Good Friday"), Is.True);
            Assert.That(holidays.Any(h => h.Name == "Family Day"), Is.True);
        }

        [Test]
        public void GetSouthAfricanPublicHolidays_HolidayOnSunday_CreatesObservedHoliday()
        {
            // Find a year where a major holiday falls on Sunday
            // Christmas 2022 was on Sunday
            var holidays = _businessDayService.GetSouthAfricanPublicHolidays(2022);

            var christmas = holidays.First(h => h.Name == "Christmas Day");
            var observedHoliday = holidays.FirstOrDefault(h => h.Name.Contains("observed") && h.Name.Contains("Christmas"));

            Assert.That(christmas.Date.DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
            Assert.That(observedHoliday, Is.Not.Null);
            Assert.That(observedHoliday.Date, Is.EqualTo(christmas.Date.AddDays(1)));
        }

        [Test]
        public async Task IsDateRangeValidForLeaveAsync_ValidRange_ReturnsTrue()
        {
            // Arrange
            var monday = new DateTime(2025, 7, 14);
            var friday = new DateTime(2025, 7, 18);

            // Act
            var result = await _businessDayService.IsDateRangeValidForLeaveAsync(monday, friday);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsDateRangeValidForLeaveAsync_StartDateWeekend_ReturnsFalse()
        {
            // Arrange
            var saturday = new DateTime(2025, 7, 19);
            var monday = new DateTime(2025, 7, 21);

            // Act
            var result = await _businessDayService.IsDateRangeValidForLeaveAsync(saturday, monday);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AdjustDatesToBusinessDaysAsync_BothBusinessDays_ReturnsUnchanged()
        {
            // Arrange
            var monday = new DateTime(2025, 7, 14);
            var friday = new DateTime(2025, 7, 18);

            // Act
            var result = await _businessDayService.AdjustDatesToBusinessDaysAsync(monday, friday);

            // Assert
            Assert.That(result.adjustedStart, Is.EqualTo(monday));
            Assert.That(result.adjustedEnd, Is.EqualTo(friday));
        }

        [Test]
        public async Task AdjustDatesToBusinessDaysAsync_StartDateWeekend_AdjustsToNextBusinessDay()
        {
            // Arrange
            var saturday = new DateTime(2025, 7, 19);
            var friday = new DateTime(2025, 7, 25);
            var expectedMonday = new DateTime(2025, 7, 21);

            // Act
            var result = await _businessDayService.AdjustDatesToBusinessDaysAsync(saturday, friday);

            // Assert
            Assert.That(result.adjustedStart, Is.EqualTo(expectedMonday));
            Assert.That(result.adjustedEnd, Is.EqualTo(friday));
        }
    }
}