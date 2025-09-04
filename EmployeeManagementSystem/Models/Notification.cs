using EmployeeManagementSystem.Models;

public class Notification
{
    public int Id { get; set; }
    public string Message { get; set; }
    public string ReceiverId { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; } = false; // default false
}
