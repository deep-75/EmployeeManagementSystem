using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.Models.ViewModels
{
    public class NotificationCreateVM
    {
        [Required(ErrorMessage = "Please select a receiver")]
        public string ReceiverId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> Employees { get; set; } = new List<SelectListItem>();
    }
}
