using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Persistence.Services;
using NSubstitute;

namespace LeaveManagement.Tests.Tests.Services;

[TestFixture]
public class AuthorizationServiceTests
{
    private IEmployeeRepositoryAsync _employeeRepository;
    private AuthorizationService _authorizationService;

    [SetUp]
    public void SetUp()
    {
        _employeeRepository = Substitute.For<IEmployeeRepositoryAsync>();
        _authorizationService = new AuthorizationService(_employeeRepository);
    }

    [Test]
    public async Task CanUserApproveLeaveRequestAsync_CEOApprover_ReturnsTrue()
    {
        // Arrange
        var employee = new Employee { EmployeeId = 1, ManagerId = 2 };
        var ceo = new Employee { EmployeeId = 3, EmployeeType = EmployeeType.CEO };

        _employeeRepository.GetByIdAsync(1).Returns(employee);
        _employeeRepository.GetByIdAsync(3).Returns(ceo);

        // Act
        var result = await _authorizationService.CanUserApproveLeaveRequestAsync(3, 1);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CanUserApproveLeaveRequestAsync_DirectManager_ReturnsTrue()
    {
        // Arrange
        var employee = new Employee { EmployeeId = 1, ManagerId = 2 };
        var manager = new Employee { EmployeeId = 2, EmployeeType = EmployeeType.Manager };

        _employeeRepository.GetByIdAsync(1).Returns(employee);
        _employeeRepository.GetByIdAsync(2).Returns(manager);

        // Act
        var result = await _authorizationService.CanUserApproveLeaveRequestAsync(2, 1);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CanUserApproveLeaveRequestAsync_UnrelatedUser_ReturnsFalse()
    {
        // Arrange
        var employee = new Employee { EmployeeId = 1, ManagerId = 2 };
        var otherEmployee = new Employee { EmployeeId = 3, EmployeeType = EmployeeType.Employee };

        _employeeRepository.GetByIdAsync(1).Returns(employee);
        _employeeRepository.GetByIdAsync(3).Returns(otherEmployee);

        // Act
        var result = await _authorizationService.CanUserApproveLeaveRequestAsync(3, 1);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CanUserApproveLeaveRequestAsync_EmployeeNotFound_ReturnsFalse()
    {
        // Arrange
        _employeeRepository.GetByIdAsync(1).Returns((Employee)null);

        // Act
        var result = await _authorizationService.CanUserApproveLeaveRequestAsync(2, 1);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsUserManagerOfEmployeeAsync_IsDirectManager_ReturnsTrue()
    {
        // Arrange
        var employee = new Employee { EmployeeId = 1, ManagerId = 2 };
        _employeeRepository.GetByIdAsync(1).Returns(employee);

        // Act
        var result = await _authorizationService.IsUserManagerOfEmployeeAsync(2, 1);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsUserManagerOfEmployeeAsync_NotDirectManager_ReturnsFalse()
    {
        // Arrange
        var employee = new Employee { EmployeeId = 1, ManagerId = 2 };
        _employeeRepository.GetByIdAsync(1).Returns(employee);
        // Act
        var result = await _authorizationService.IsUserManagerOfEmployeeAsync(3, 1);

        // Assert
        Assert.That(result, Is.False);
    }
}