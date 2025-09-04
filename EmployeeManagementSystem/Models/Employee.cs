using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Position { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public Department? Department { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Salary { get; set; }
    }
}
