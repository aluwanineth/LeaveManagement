using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Wrappers;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Text;
using System.Text.Json;

namespace LeaveManagement.Tests.Tests.Integration;

[TestFixture]
public class LeaveRequestIntegrationTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private IServiceScope _scope; // Declare a scope variable to manage its lifecycle
    private ApplicationDbContext _context;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });

        _client = _factory.CreateClient();

        // Create the scope and get the context here
        _scope = _factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client?.Dispose();
        _factory?.Dispose();
        _scope?.Dispose();
        _context?.Dispose();
    }

    [SetUp]
    public async Task SetUp()
    {
        await SeedTestData();
    }

    [TearDown]
    public async Task TearDown()
    {
        _context.LeaveRequests.RemoveRange(_context.LeaveRequests);
        _context.Employees.RemoveRange(_context.Employees);
        await _context.SaveChangesAsync();
    }

    [Test]
    public async Task GetLeaveRequestsByEmployee_ExistingEmployee_ReturnsRequests()
    {
        // Arrange
        var employeeId = 1;

        // Act
        var response = await _client.GetAsync($"/api/v1/leaverequest/employee/{employeeId}");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Response<List<LeaveRequestResponse>>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.That(result.Succeeded, Is.True);
        Assert.That(result.Data.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task CreateLeaveRequest_ValidRequest_CreatesRequest()
    {
        // Arrange
        var request = new LeaveRequestRequest
        {
            EmployeeId = 1,
            StartDate = DateTime.Today.AddDays(10),
            EndDate = DateTime.Today.AddDays(15),
            LeaveType = LeaveType.Annual,
            Comments = "Integration test vacation"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/leaverequest", content);

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Response<LeaveRequestResponse>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.That(result.Succeeded, Is.True);
        Assert.That(result.Data.EmployeeId, Is.EqualTo(1));
        Assert.That(result.Data.LeaveType, Is.EqualTo(LeaveType.Annual));
    }

    private async Task SeedTestData()
    {
        var manager = new Employee
        {
            EmployeeId = 2,
            EmployeeNumber = "MGR001",
            FullName = "Test Manager",
            Email = "manager@test.com",
            EmployeeType = EmployeeType.Manager
        };

        var employee = new Employee
        {
            EmployeeId = 1,
            EmployeeNumber = "EMP001",
            FullName = "Test Employee",
            Email = "employee@test.com",
            EmployeeType = EmployeeType.Employee,
            ManagerId = 2
        };

        var leaveRequest = new LeaveRequest
        {
            EmployeeId = 1,
            StartDate = DateTime.Today.AddDays(5),
            EndDate = DateTime.Today.AddDays(10),
            LeaveType = LeaveType.Annual,
            Status = LeaveStatus.Pending,
            Comments = "Test leave request"
        };

        _context.Employees.AddRange(manager, employee);
        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync();
    }
}