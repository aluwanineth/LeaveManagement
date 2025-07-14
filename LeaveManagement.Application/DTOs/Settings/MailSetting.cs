namespace LeaveManagement.Application.DTOs.Settings;

public record MailSetting
{
    public string EmailFrom { get; set; } = string.Empty;
    public string EmailTo { get; set; } = string.Empty;
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587; // Default SMTP port for TLS
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
