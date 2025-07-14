using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;

namespace LeaveManagement.Tests.Tests.Domain;

[TestFixture]
public class LeaveRequestTests
{
    [Test]
    public void LeaveRequest_Creation_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var startDate = DateTime.Today.AddDays(10);
        var endDate = DateTime.Today.AddDays(15);

        // Act
        var leaveRequest = new LeaveRequest
        {
            LeaveRequestId = 1,
            EmployeeId = 1,
            StartDate = startDate,
            EndDate = endDate,
            LeaveType = LeaveType.Annual,
            Comments = "Family vacation",
            Status = LeaveStatus.Pending
        };

        // Assert
        Assert.That(leaveRequest.LeaveRequestId, Is.EqualTo(1));
        Assert.That(leaveRequest.EmployeeId, Is.EqualTo(1));
        Assert.That(leaveRequest.StartDate, Is.EqualTo(startDate));
        Assert.That(leaveRequest.EndDate, Is.EqualTo(endDate));
        Assert.That(leaveRequest.LeaveType, Is.EqualTo(LeaveType.Annual));
        Assert.That(leaveRequest.Comments, Is.EqualTo("Family vacation"));
        Assert.That(leaveRequest.Status, Is.EqualTo(LeaveStatus.Pending));
    }

    [Test]
    public void LeaveRequest_ApprovalProcess_ShouldUpdateCorrectly()
    {
        // Arrange
        var leaveRequest = new LeaveRequest
        {
            Status = LeaveStatus.Pending
        };
        var approvalDate = DateTime.Now;

        // Act
        leaveRequest.Status = LeaveStatus.Approved;
        leaveRequest.ApprovedById = 2;
        leaveRequest.ApprovedDate = approvalDate;
        leaveRequest.ApprovalComments = "Approved for vacation";

        // Assert
        Assert.That(leaveRequest.Status, Is.EqualTo(LeaveStatus.Approved));
        Assert.That(leaveRequest.ApprovedById, Is.EqualTo(2));
        Assert.That(leaveRequest.ApprovedDate, Is.EqualTo(approvalDate));
        Assert.That(leaveRequest.ApprovalComments, Is.EqualTo("Approved for vacation"));
    }
}
