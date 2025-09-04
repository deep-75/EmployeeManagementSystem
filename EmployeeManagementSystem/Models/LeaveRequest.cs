using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;

        public AppUser? Employee { get; set; }   // ✅ navigation property

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public string LeaveType { get; set; } = string.Empty;
    }
}
