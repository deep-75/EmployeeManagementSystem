namespace EmployeeManagementSystem.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }      // Identity User Id
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string Role { get; set; }
        public string? FullName { get; set; }
    }
}
