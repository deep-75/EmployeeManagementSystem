using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; } = string.Empty;

        [ForeignKey("EmployeeId")]
        public AppUser? Employee { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now.Date;

        [DataType(DataType.Time)]
        public DateTime? CheckInTime { get; set; }

        [DataType(DataType.Time)]
        public DateTime? CheckOutTime { get; set; }

        [StringLength(20)]
        public string? DayType { get; set; }   // "Full Day" or "Half Day"
    }

}
