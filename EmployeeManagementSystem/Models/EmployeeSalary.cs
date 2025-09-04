using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    namespace EmployeeManagementSystem.Models
    {
        public class EmployeeSalary
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public string EmployeeId { get; set; } = string.Empty;

            [ForeignKey("EmployeeId")]
            public AppUser Employee { get; set; }

            [Required]
            [Range(0, double.MaxValue, ErrorMessage = "Salary must be greater than or equal to 0")]
            public decimal BaseSalary { get; set; }

            [Range(0, double.MaxValue)]
            public decimal Bonus { get; set; } = 0;

            [Range(0, double.MaxValue)]
            public decimal Deductions { get; set; } = 0;

            [Required]
            [DataType(DataType.Date)]
            public DateTime EffectiveFrom { get; set; } = DateTime.Now.Date;
        [Required]
        public decimal SalaryAmount { get; set; }
        [NotMapped]
            public decimal NetSalary => BaseSalary + Bonus - Deductions;
        }
    }