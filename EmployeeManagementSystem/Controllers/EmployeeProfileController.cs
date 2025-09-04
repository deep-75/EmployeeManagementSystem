using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Data;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize(Roles ="Employee")]
    public class EmployeeProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public EmployeeProfileController(AppDbContext context, UserManager<AppUser> userManager)
        {
          _context = context;
            _userManager = userManager;
        }

        //Get: EmployeeProfile
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            // Link app user withh employee

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Email == user.Email);
            if (employee == null) return NotFound("Employee Record not found for logged in user,");

            return View(employee);
        }
    }
}
