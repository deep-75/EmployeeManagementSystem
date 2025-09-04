using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize(Roles = "Employee,Admin")]
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public EmployeesController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .ToListAsync();

            return View(employees);
        }

        // Get: Employees
        public async Task<IActionResult> Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        //Post Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Salary,DepartmentId,Position")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            return View(employee);
        }

        //Get: Employee edit
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var employee = _context.Employees.Find(id);
            if (employee == null) return NotFound();

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            return View(employee);
        }

        //Post Edit Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Edit(int? id,Employee employee)
        {
            if(id == null) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if (!_context.Employees.Any(e => e.Id == employee.Id))
                        return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments,"Id","Name",employee.DepartmentId);
            return View(employee);
        }

        // Get: Delete/ Employee
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null) return NotFound();

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null) return NotFound();
            return View(employee);
        }

        // Post Delete/ Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Employee Attendance (only Employees can see their own)
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Attendance()
        {
            var user = await _userManager.GetUserAsync(User);

            var attendance = await _context.Attendances
                .Where(a => a.EmployeeId == user.Id)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View("Attendance", attendance);
        }

        // Admin Attendance (only Admin can see all)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminAttendance()
        {
            var allAttendance = await _context.Attendances
                .Include(a => a.Employee)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View("AdminAttendance", allAttendance);
        }
    }
}
