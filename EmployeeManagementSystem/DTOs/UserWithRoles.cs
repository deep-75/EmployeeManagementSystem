namespace EmployeeManagementSystem.DTOs
{
    public class UserWithRoles
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }
        public List<string> Roles { get; set; } 
    }
}
