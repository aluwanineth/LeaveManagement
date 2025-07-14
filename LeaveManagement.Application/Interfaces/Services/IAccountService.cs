using LeaveManagement.Application.DTOs.Account;
using LeaveManagement.Application.Wrappers;

namespace LeaveManagement.Application.Interfaces.Services;

public interface IAccountService
{
    Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request);
    Task<Response<string>> RegisterAsync(RegisterRequest request);
}
