using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.LeaveRequest.Commands;

public class CreateLeaveRequestCommand : IRequest<Response<LeaveRequestResponse>>
{
    public LeaveRequestRequest LeaveRequest { get; set; } = null!;
}
