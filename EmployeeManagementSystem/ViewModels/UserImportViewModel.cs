using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.ViewModels
{
    public class UserImportViewModel
    {
        public string EmployeeId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = "Employee";

        public IEnumerable<SelectListItem>? Employees { get; set; }

        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}
