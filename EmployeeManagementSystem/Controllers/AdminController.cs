using EmployeeManagementSystem.DTOs;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Models.ViewModels;
using EmployeeManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Data;
namespace EmployeeManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _context = context;
        }

        // Show all users with their roles
        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "None",
                    FullName = user.FullName,
                });
            }

            return View(model);
        }

        // ✅ Edit user (GET)
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? "None"
            };

            return View(model);
        }

        // ✅ Edit user (POST)
        [HttpPost]
        public async Task<IActionResult> EditUser(UserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.UserName = model.UserName;
            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "User updated successfully!";
                return RedirectToAction("ManageUsers");
            }

            ModelState.AddModelError("", "Failed to update user");
            return View(model);
        }

        // ✅ Change Role
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string id, string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            TempData["Success"] = "Role updated successfully!";
            return RedirectToAction("ManageUsers");
        }

        // ✅ Delete user
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                TempData["Success"] = "User deleted successfully!";
            else
                TempData["Error"] = "Failed to delete user.";

            return RedirectToAction("ManageUsers");
        }
        [HttpGet]
        public IActionResult ImportUser()
        {
            var model = new UserImportViewModel
            {
                Employees = _context.Employees
                    .Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.Name
                    })
                    .ToList(),

                Roles = _roleManager.Roles
                    .Select(r => new SelectListItem
                    {
                        Value = r.Name,
                        Text = r.Name
                    })
                    .ToList()
            };

            return View(model);
        }
        // POST: Import user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportUser(UserImportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Employees = _context.Employees.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                }).ToList();

                model.Roles = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList();

                return View(model);
            }

            var employee = await _context.Employees.FindAsync(int.Parse(model.EmployeeId));
            if (employee == null)
            {
                ModelState.AddModelError("", "Employee not found.");
                return View(model);
            }

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = employee.Name // if you have FullName in AppUser
            };

            var result = await _userManager.CreateAsync(user, "Default@123"); // default password
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                TempData["Success"] = "User imported successfully!";
                return RedirectToAction("ManageUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }
}

