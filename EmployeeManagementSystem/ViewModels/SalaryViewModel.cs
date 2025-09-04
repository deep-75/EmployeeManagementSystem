using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.ViewModels
{
    public class SalaryViewModel
    {
        public int SalaryId { get; set; }

        [Required]
        public string EmployeeId { get; set; } // match type

        public string EmployeeName { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal BaseSalary { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Allowances { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Deductions { get; set; }

        public decimal NetSalary => BaseSalary + Allowances - Deductions;
    }

}
