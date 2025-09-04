using Microsoft.AspNetCore.Mvc.Rendering;
namespace EmployeeManagementSystem.Models.ViewModels;
public class NotificationViewModel
{
    public string Message { get; set; } = string.Empty;

    // selected employee
    public string ReceiverId { get; set; } = string.Empty;

    // dropdown list
    public IEnumerable<SelectListItem> Employees { get; set; } = new List<SelectListItem>();
}
