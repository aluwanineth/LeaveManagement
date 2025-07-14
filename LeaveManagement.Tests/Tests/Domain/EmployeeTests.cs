using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;

namespace LeaveManagement.Tests.Tests.Domain;

[TestFixture]
public class EmployeeTests
{
    [Test]
    public void Employee_Creation_ShouldSetPropertiesCorrectly()
    {
        // Arrange & Act
        var employee = new Employee
        {
            EmployeeId = 1,
            EmployeeNumber = "EMP001",
            FullName = "John Doe",
            Email = "john.doe@acme.com",
            CellphoneNumber = "+1234567890",
            EmployeeType = EmployeeType.Employee,
            ManagerId = 2
        };

        // Assert
        Assert.That(employee.EmployeeId, Is.EqualTo(1));
        Assert.That(employee.EmployeeNumber, Is.EqualTo("EMP001"));
        Assert.That(employee.FullName, Is.EqualTo("John Doe"));
        Assert.That(employee.Email, Is.EqualTo("john.doe@acme.com"));
        Assert.That(employee.CellphoneNumber, Is.EqualTo("+1234567890"));
        Assert.That(employee.EmployeeType, Is.EqualTo(EmployeeType.Employee));
        Assert.That(employee.ManagerId, Is.EqualTo(2));
    }

    [Test]
    public void Employee_WithoutManager_ShouldAllowNullManagerId()
    {
        // Arrange & Act
        var ceo = new Employee
        {
            EmployeeId = 1,
            EmployeeNumber = "CEO001",
            FullName = "CEO Name",
            Email = "ceo@acme.com",
            EmployeeType = EmployeeType.CEO,
            ManagerId = null
        };

        // Assert
        Assert.That(ceo.ManagerId, Is.Null);
        Assert.That(ceo.EmployeeType, Is.EqualTo(EmployeeType.CEO));
    }
}
