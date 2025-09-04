// Models/ViewModels/TaskCreateVM.cs
using System.ComponentModel.DataAnnotations;

public class TaskCreateVM
{
    [Required]
    public string Title { get; set; }

    public string? Description { get; set; }

    [Required]
    public string AssignedToUserId { get; set; }

    public DateTime? DueDate { get; set; }
}
