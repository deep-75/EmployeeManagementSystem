using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }
    }
}
