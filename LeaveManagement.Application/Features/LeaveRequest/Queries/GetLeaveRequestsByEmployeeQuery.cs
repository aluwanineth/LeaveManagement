using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.LeaveRequest.Queries;

public class GetLeaveRequestsByEmployeeQuery : IRequest<Response<IReadOnlyList<LeaveRequestResponse>>>
{
    public int EmployeeId { get; set; }
}
