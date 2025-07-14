namespace LeaveManagement.Domain.Common;

public abstract class AuditableBaseEntity
{
    public DateTime? LastRecordUpdateDate { get; set; }
    public string? LastRecordUpdateUserid { get; set; }
}
