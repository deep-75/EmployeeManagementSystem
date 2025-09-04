using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.ViewModels
{
    public class TaskCommentCreateVM
    {
        [Required]
        public int TaskItemId { get; set; }

        [Required, StringLength(1000)]
        public string CommentText { get; set; }
    }
}
