using Microsoft.EntityFrameworkCore;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Persistence.Contexts;
using LeaveManagement.Application.Interfaces.Repositories;

namespace LeaveManagement.Persistence.Repositories;

public class LeaveRequestRepositoryAsync(ApplicationDbContext dbContext) : GenericRepositoryAsync<LeaveRequest>(dbContext), ILeaveRequestRepositoryAsync
{
    public async Task<IReadOnlyList<LeaveRequest>> GetByEmployeeIdAsync(int employeeId)
    {
        return await dbContext.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.ApprovedBy)
            .Where(lr => lr.EmployeeId == employeeId)
            .OrderByDescending(lr => lr.StartDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<LeaveRequest>> GetPendingRequestsForManagerAsync(int managerId)
    {
        var subordinateIds = await GetAllSubordinateIdsAsync(managerId);

        return await dbContext.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.ApprovedBy)
            .Where(lr => subordinateIds.Contains(lr.EmployeeId) && lr.Status == LeaveStatus.Pending)
            .OrderBy(lr => lr.StartDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<LeaveRequest>> GetLeaveRequestsForApprovalAsync(int approverId)
    {
        var subordinateIds = await GetAllSubordinateIdsAsync(approverId);

        return await dbContext.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.ApprovedBy)
            .Where(lr => subordinateIds.Contains(lr.EmployeeId))
            .OrderByDescending(lr => lr.StartDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<LeaveRequest>> GetPendingApprovalsAsync(int approverId)
    {
        return await GetPendingRequestsForManagerAsync(approverId);
    }
    private async Task<List<int>> GetAllSubordinateIdsAsync(int managerId)
    {
        var subordinateIds = new List<int>();

        var directReports = await dbContext.Employees
            .Where(e => e.ManagerId == managerId)
            .Select(e => e.EmployeeId)
            .ToListAsync();

        subordinateIds.AddRange(directReports);

        foreach (var directReportId in directReports)
        {
            var indirectReports = await dbContext.Employees
                .Where(e => e.ManagerId == directReportId)
                .Select(e => e.EmployeeId)
                .ToListAsync();

            subordinateIds.AddRange(indirectReports);
        }

        return subordinateIds;
    }

    public async Task<LeaveRequest?> GetLeaveRequestWithDetailsAsync(int leaveRequestId)
    {
        return await dbContext.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.ApprovedBy)
            .FirstOrDefaultAsync(lr => lr.LeaveRequestId == leaveRequestId);
    }

    public async Task<bool> HasPendingLeaveRequestsAsync(int employeeId, DateTime startDate, DateTime endDate)
    {
        return await dbContext.LeaveRequests
            .AnyAsync(lr => lr.EmployeeId == employeeId &&
                           lr.Status == LeaveStatus.Pending &&
                           ((lr.StartDate <= endDate && lr.EndDate >= startDate)));
    }

    public async Task<IReadOnlyList<LeaveRequest>> GetLeaveRequestsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await dbContext.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.ApprovedBy)
            .Where(lr => lr.StartDate >= startDate && lr.EndDate <= endDate)
            .OrderBy(lr => lr.StartDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<LeaveRequest>> GetLeaveRequestsByStatusAsync(LeaveStatus status)
    {
        return await dbContext.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.ApprovedBy)
            .Where(lr => lr.Status == status)
            .OrderByDescending(lr => lr.StartDate)
            .ToListAsync();
    }

    public async Task<int> GetPendingRequestCountForManagerAsync(int managerId)
    {
        var subordinateIds = await GetAllSubordinateIdsAsync(managerId);

        return await dbContext.LeaveRequests
            .CountAsync(lr => subordinateIds.Contains(lr.EmployeeId) && lr.Status == LeaveStatus.Pending);
    }
}