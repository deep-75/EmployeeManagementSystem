using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.ViewModels
{
    public class EmployeeSalaryViewModel
    {
        public int Id { get; set; }

        [Required]
        public string? EmployeeId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal BaseSalary { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Bonus { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal Deductions { get; set; } = 0;

        [Required]
        [DataType(DataType.Date)]
        public DateTime EffectiveFrom { get; set; } = DateTime.Now.Date;

        public decimal NetSalary => BaseSalary + Bonus - Deductions;

        // For dropdown in Create/Edit forms
        public IEnumerable<SelectListItem> Employees { get; set; } = Enumerable.Empty<SelectListItem>();

        [Required]
        public decimal SalaryAmount { get; set; }
    }
}
