using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Persistence.Repositories
{
    public class EmployeeRepositoryAsync(ApplicationDbContext dbContext) : GenericRepositoryAsync<Employee>(dbContext), IEmployeeRepositoryAsync
    {
        public async Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber)
        {
            return await dbContext.Employees
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber);
        }

        public async Task<IReadOnlyList<Employee>> GetSubordinatesAsync(int managerId)
        {
            return await dbContext.Employees
                .Where(e => e.ManagerId == managerId)
                .ToListAsync();
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await dbContext.Employees
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.Email == email);
        }
    }
}
