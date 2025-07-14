namespace LeaveManagement.Application.DTOs.Account;

public class AuthenticationResponse
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string JWToken { get; set; } = string.Empty;
    public int? EmployeeId { get; set; }
}
