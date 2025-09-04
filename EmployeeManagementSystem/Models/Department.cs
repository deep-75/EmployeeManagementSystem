namespace EmployeeManagementSystem.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
