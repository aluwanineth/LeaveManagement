namespace LeaveManagement.Application.DTOs.Settings;

public record JWTSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public double DurationInMinutes { get; set; } = 60; // Default to 1 hour
}
