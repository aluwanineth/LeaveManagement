using LeaveManagement.Domain.Entities;

namespace LeaveManagement.Application.Interfaces.Repositories;

public interface IEmployeeRepositoryAsync : IGenericRepositoryAsync<Employee>
{
    Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber);
    Task<IReadOnlyList<Employee>> GetSubordinatesAsync(int managerId);
    Task<Employee?> GetByEmailAsync(string email);
}
