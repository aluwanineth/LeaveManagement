using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Identity.Contexts;
using LeaveManagement.Identity.Models;
using LeaveManagement.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Data;

public static class SeedData
{
    public static async Task SeedAsync(
        ApplicationDbContext applicationContext,
        IdentityContext identityContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Seed Roles first
        await SeedRolesAsync(roleManager);

        // Seed Employees (business data)
        await SeedEmployeesAsync(applicationContext);

        // Seed Users (identity data)
        await SeedUsersAsync(userManager, applicationContext);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "CEO", "Manager", "TeamLead", "Employee" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedEmployeesAsync(ApplicationDbContext context)
    {
        if (await context.Employees.AnyAsync())
            return;

        var employees = new List<Employee>
            {
                // CEO
                new Employee
                {
                    EmployeeNumber = "0001",
                    FullName = "Linda Jenkins",
                    Email = "lindajenkins@acme.com",
                    EmployeeType = EmployeeType.CEO,
                    ManagerId = null
                },
                
                // Management Team
                new Employee
                {
                    EmployeeNumber = "0002",
                    FullName = "Milton Coleman",
                    Email = "miltoncoleman@acme.com", // Fixed typo from original
                    CellphoneNumber = "+27 55 937 274",
                    EmployeeType = EmployeeType.Manager,
                    ManagerId = 1 // Linda Jenkins
                },
                new Employee
                {
                    EmployeeNumber = "0003",
                    FullName = "Colin Horton",
                    Email = "colinhorton@acme.com", // Fixed typo from original
                    CellphoneNumber = "+27 20 915 7545",
                    EmployeeType = EmployeeType.TeamLead,
                    ManagerId = 1 // Linda Jenkins
                },
                
                // Support Team (under Milton Coleman)
                new Employee
                {
                    EmployeeNumber = "1005",
                    FullName = "Charlotte Osborne",
                    Email = "charlotteosborne@acme.com",
                    CellphoneNumber = "+27 55 760 177",
                    EmployeeType = EmployeeType.Employee,
                    ManagerId = 2 // Milton Coleman
                },
                new Employee
                {
                    EmployeeNumber = "1006",
                    FullName = "Marie Walters",
                    Email = "mariewalters@acme.com",
                    CellphoneNumber = "+27 20 918 6908",
                    EmployeeType = EmployeeType.Employee,
                    ManagerId = 2 // Milton Coleman
                },
                new Employee
                {
                    EmployeeNumber = "1008",
                    FullName = "Leonard Gill",
                    Email = "leonardgill@acme.com",
                    CellphoneNumber = "+27 55 525 585",
                    EmployeeType = EmployeeType.Employee,
                    ManagerId = 2 // Milton Coleman
                },
                new Employee
                {
                    EmployeeNumber = "1009",
                    FullName = "Enrique Thomas",
                    Email = "enriquethomas@acme.com",
                    CellphoneNumber = "+27 20 916 1335",
                    EmployeeType = EmployeeType.Employee,
                    ManagerId = 2 // Milton Coleman
                },
                new Employee
                {
                    EmployeeNumber = "1010",
                    FullName = "Omar Dunn",
                    Email = "omardunn@acme.com",
                    EmployeeType = EmployeeType.Employee,
                    ManagerId = 2 // Milton Coleman
                },
                new Employee
                {
                    EmployeeNumber = "1012",
                    FullName = "Dewey George",
                    Email = "deweygeorge@acme.com",
                    CellphoneNumber = "+27 55 260 127",
                    EmployeeType = EmployeeType.Employee,
                    ManagerId = 2 // Milton Coleman
                },
                new Employee
                {
                    EmployeeNumber = "1013",
                    FullName = "Rudy Lewis",
                    Email = "rudylewis@acme.com",
                    EmployeeType = EmployeeType.Employee,
                    ManagerId = 2 // Milton Coleman
                },
                new Employee
                {
                    EmployeeNumber = "1015",
                    FullName = "Neal French",
                    Email = "nealfrench@acme.com",
                    CellphoneNumber = "+27 20 919 4882",
                    EmployeeType = EmployeeType.Employee,
                    ManagerId = 2 // Milton Coleman
                },
                
                // Dev Team (under Colin Horton)
                new Employee
                {
                    EmployeeNumber = "2005",
                    FullName = "Ella Jefferson",
                    Email = "ellajefferson@acme.com",
                    CellphoneNumber = "+27 55 979 367",
                    EmployeeType = EmployeeType.Employee,
                    ManagerId = 3 // Colin Horton
                },
                new Employee
                {
                    EmployeeNumber = "2006",
                    FullName = "Earl Craig",
                    Email = "earlcraig@acme.com",
                    CellphoneNumber = "+27 20 916 5608",
                    EmployeeType = EmployeeType.Employee,
                    ManagerId = 3 // Colin Horton
                },
                new Employee
                {
                    EmployeeNumber = "2008",
                    FullName = "Marsha Murphy",
                    Email = "marshamurphy@acme.com",
                    CellphoneNumber = "+36 55 949 891",
                    EmployeeType = EmployeeType.Employee,
                    ManagerId = 3 // Colin Horton
                },
                new Employee
                {
                    EmployeeNumber = "2009",
                    FullName = "Luis Ortega",
                    Email = "luisortega@acme.com",
                    CellphoneNumber = "+27 20 917 1339",
                    EmployeeType = EmployeeType.Employee,
                    ManagerId = 3 // Colin Horton
                },
                new Employee
                {
                    EmployeeNumber = "2010",
                    FullName = "Faye Dennis",
                    Email = "fayedennis@acme.com",
                    EmployeeType = EmployeeType.Employee,
                    ManagerId = 3 // Colin Horton
                },
                new Employee
                {
                    EmployeeNumber = "2012",
                    FullName = "Amy Burns",
                    Email = "amyburns@acme.com",
                    CellphoneNumber = "+27 20 914 1775",
                    EmployeeType = EmployeeType.Employee,
                    ManagerId = 3 // Colin Horton
                },
                new Employee
                {
                    EmployeeNumber = "2013",
                    FullName = "Darrel Weber",
                    Email = "darrelweber@acme.com",
                    CellphoneNumber = "+27 55 615 463",
                    EmployeeType = EmployeeType.Employee,
                    ManagerId = 3 // Colin Horton
                }
            };

        await context.Employees.AddRangeAsync(employees);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationContext)
    {
        var employees = await applicationContext.Employees.ToListAsync();

        foreach (var employee in employees)
        {
            var userName = employee.Email.Split('@')[0];

            if (await userManager.FindByEmailAsync(employee.Email) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = employee.Email,
                    FirstName = employee.FullName.Split(' ')[0],
                    LastName = employee.FullName.Split(' ').Last(),
                    EmployeeId = employee.EmployeeId,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Password123!");

                if (result.Succeeded)
                {
                    var roleName = GetRoleNameByEmployeeType(employee.EmployeeType);
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }
    }

    private static string GetRoleNameByEmployeeType(EmployeeType employeeType)
    {
        return employeeType switch
        {
            EmployeeType.CEO => "CEO",
            EmployeeType.Manager => "Manager",
            EmployeeType.TeamLead => "TeamLead",
            EmployeeType.Employee => "Employee",
            _ => "Employee"
        };
    }
}