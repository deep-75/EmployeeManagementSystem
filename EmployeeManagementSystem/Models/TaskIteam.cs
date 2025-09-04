// Models/TaskItem.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public DateTime? DueDate { get; set; }

        [Required]
        public string AssignedToUserId { get; set; }

        [ForeignKey("AssignedToUserId")]
        public AppUser AssignedTo { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed
        public bool IsNotified { get; set; } = false;
        public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    }


}
