using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public TasksController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Admin - View All Tasks
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.Tasks.Include(t => t.AssignedTo).ToListAsync();
            return View(tasks);
        }

        // Employee - My Tasks
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> MyTasks()
        {
            var user = await _userManager.GetUserAsync(User);
            var tasks = await _context.Tasks
                .Where(t => t.AssignedToUserId == user.Id)
                .ToListAsync();
            return View(tasks);
        }

        // GET
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Users = new SelectList(_userManager.Users.ToList(), "Id", "UserName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(TaskCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = new SelectList(_userManager.Users.ToList(), "Id", "UserName", model.AssignedToUserId);
                return View(model);
            }

            var task = new TaskItem
            {
                Title = model.Title,
                Description = model.Description,
                AssignedToUserId = model.AssignedToUserId,
                DueDate = model.DueDate
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Emoloyee Admin Update status
        [HttpPost]
        [Authorize (Roles ="Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>UpdateStatus(int id, string status)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if(User.IsInRole("Employee") && task.AssignedToUserId == user.Id)
            {
                task.Status = status;
                _context.Update(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "You are not allowed to update the task status.";
            return RedirectToAction(nameof(Index));

        }

        // Employee - Mark Task Complete
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Complete(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (task.AssignedToUserId != user.Id) return Forbid();

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyTasks));
        }
       
        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }
    }
}
