using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize] // both Admin & Employee can access; we guard per-item within actions
    public class TaskCommentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public TaskCommentsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // LIST: Admin sees all comments; Employee sees comments only on tasks assigned to them
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = User.IsInRole("Admin");

            IQueryable<TaskComment> query = _context.TaskComments
                .Include(c => c.User)
                .Include(c => c.TaskItem);

            if (!isAdmin)
            {
                query = query.Where(c => c.TaskItem.AssignedToUserId == user.Id);
            }

            var comments = await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(comments);
        }

        // GET: Create comment
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = User.IsInRole("Admin");

            // Admin: can comment on any task. Employee: only on their own tasks
            var tasks = isAdmin
                ? await _context.Tasks.OrderBy(t => t.Title).ToListAsync()
                : await _context.Tasks.Where(t => t.AssignedToUserId == user.Id)
                                      .OrderBy(t => t.Title).ToListAsync();

            ViewBag.TaskItemId = new SelectList(tasks, "Id", "Title");
            return View(new TaskCommentCreateVM());
        }

        // POST: Create comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskCommentCreateVM vm)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = User.IsInRole("Admin");

            if (!ModelState.IsValid)
            {
                var tasksReload = isAdmin
                    ? await _context.Tasks.OrderBy(t => t.Title).ToListAsync()
                    : await _context.Tasks.Where(t => t.AssignedToUserId == user.Id)
                                          .OrderBy(t => t.Title).ToListAsync();

                ViewBag.TaskItemId = new SelectList(tasksReload, "Id", "Title", vm.TaskItemId);
                return View(vm);
            }

            // Security: ensure employee can only comment on their tasks
            var task = await _context.Tasks.FindAsync(vm.TaskItemId);
            if (task == null) return NotFound();

            if (!isAdmin && task.AssignedToUserId != user.Id)
                return Forbid();

            var comment = new TaskComment
            {
                TaskItemId = vm.TaskItemId,
                UserId = user.Id,
                CommentText = vm.CommentText,
                CreatedAt = DateTime.Now
            };

            _context.TaskComments.Add(comment);
            await _context.SaveChangesAsync();

            // Redirect to a discussion place; using Index for simplicity
            return RedirectToAction(nameof(Index));
        }

        // GET: Details (view single comment)
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = User.IsInRole("Admin");

            var comment = await _context.TaskComments
                .Include(c => c.User)
                .Include(c => c.TaskItem)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null) return NotFound();

            if (!isAdmin && comment.TaskItem.AssignedToUserId != user.Id)
                return Forbid();

            return View(comment);
        }
        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.Comments) // include comments
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return NotFound();

            return View(task); // show confirmation page
        }
        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.Comments) // include comments
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return NotFound();

            // delete all comments manually first
            if (task.Comments.Any())
            {
                _context.TaskComments.RemoveRange(task.Comments);
            }

            // now delete the task
            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Task and its comments were deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
