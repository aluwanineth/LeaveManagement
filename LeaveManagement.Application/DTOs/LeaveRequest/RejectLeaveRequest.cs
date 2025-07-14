using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Application.DTOs.LeaveRequest;

public class RejectLeaveRequest
{
    public int LeaveRequestId { get; set; }

    [Required(ErrorMessage = "Rejection comments are required")]
    [StringLength(500, ErrorMessage = "Rejection comments cannot exceed 500 characters")]
    public string RejectionComments { get; set; } = string.Empty;
}
