using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public class TaskComment
    {
        public int Id { get; set; }

        [Required]
        public int TaskItemId { get; set; }

        [ForeignKey(nameof(TaskItemId))]
        public TaskItem TaskItem { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }

        [Required, StringLength(1000)]
        public string CommentText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
