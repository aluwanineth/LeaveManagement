using AutoMapper;
using LeaveManagement.Application.DTOs.Employee;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Domain.Entities;

namespace LeaveManagement.Application.Mappings;

public class GeneralProfile : Profile
{
    public GeneralProfile()
    {
        CreateMap<Employee, EmployeeResponse>()
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : null));

        CreateMap<EmployeeRequest, Employee>();

        CreateMap<LeaveRequest, LeaveRequestResponse>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
            .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src => src.ApprovedBy != null ? src.ApprovedBy.FullName : null));

        CreateMap<LeaveRequestRequest, LeaveRequest>();
    }
}
