using AutoMapper;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Application.Wrappers;
using LeaveManagement.Domain.Enums;
using MediatR;

namespace LeaveManagement.Application.Features.LeaveRequest.Commands;

public class CreateLeaveRequestCommandHandler(ILeaveRequestRepositoryAsync leaveRequestRepository, IMapper mapper) : IRequestHandler<CreateLeaveRequestCommand, Response<LeaveRequestResponse>>
{
    public async Task<Response<LeaveRequestResponse>> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var leaveRequest = mapper.Map<Domain.Entities.LeaveRequest>(request.LeaveRequest);
        leaveRequest.Status = LeaveStatus.Pending;

        var createdLeaveRequest = await leaveRequestRepository.AddAsync(leaveRequest);
        var leaveRequestResponse = mapper.Map<LeaveRequestResponse>(createdLeaveRequest);

        return new Response<LeaveRequestResponse>(leaveRequestResponse, "Leave request created successfully");
    }
}
