using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.ViewModels;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    public class EmployeeSalaryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public EmployeeSalaryController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Admin: View all salaries
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var salaries = await _context.EmployeeSalaries
                .Include(s => s.Employee)
                .ToListAsync();

            return View(salaries);
        }

        // Employee: View my salary
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> MySalary()
        {
            var user = await _userManager.GetUserAsync(User);
            var salaries = await _context.EmployeeSalaries
                .Where(s => s.EmployeeId == user!.Id)
                .OrderByDescending(s => s.EffectiveFrom)
                .ToListAsync();

            return View(salaries);
        }

        // Admin: Create salary
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new EmployeeSalaryViewModel
            {
                Employees = _userManager.Users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.UserName
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(EmployeeSalaryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Employees = _userManager.Users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.UserName
                }).ToList();
                return View(model);
            }

            var salary = new EmployeeSalary
            {
                EmployeeId = model.EmployeeId!,
                BaseSalary = model.BaseSalary,
                Bonus = model.Bonus,
                Deductions = model.Deductions,
                EffectiveFrom = model.EffectiveFrom
            };

            _context.EmployeeSalaries.Add(salary);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Salary allocated successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
