using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public class Payroll
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; } = string.Empty;

        [ForeignKey("EmployeeId")]
        public AppUser Employee { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Month { get; set; } // store first day of month e.g., 2025-08-01

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseSalary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Deductions { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetSalary { get; set; }
    }
}
