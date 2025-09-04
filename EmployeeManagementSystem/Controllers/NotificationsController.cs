using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public NotificationsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ===== Admin: Send Notification =====
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new NotificationCreateVM
            {
                Employees = _userManager.Users
                    .Select(u => new SelectListItem { Value = u.Id, Text = string.IsNullOrWhiteSpace(u.FullName) ? u.UserName : u.FullName })
                    .ToList()
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NotificationCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Employees = _userManager.Users
                    .Select(u => new SelectListItem { Value = u.Id, Text = string.IsNullOrWhiteSpace(u.FullName) ? u.UserName : u.FullName })
                    .ToList();
                return View(vm);
            }

            var notification = new Notification
            {
                ReceiverId = vm.ReceiverId,
                Message = vm.Message,
                CreatedAt = System.DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Notification sent successfully.";
            return RedirectToAction(nameof(Create));
        }

        // ===== Employee/Admin: See my notifications =====
        [Authorize(Roles = "Employee,Admin")]
        [HttpGet]
        public async Task<IActionResult> MyNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var items = await _context.Notifications
                .Where(n => n.ReceiverId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationItemVM
                {
                    Id = n.Id,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                })
                .ToListAsync();

            return View(items);
        }

        // ===== Employee/Admin: Mark one as read =====
        [Authorize(Roles = "Employee,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var n = await _context.Notifications
                .FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == userId);

            if (n == null) return NotFound();

            n.IsRead = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyNotifications));
        }
    }
}
