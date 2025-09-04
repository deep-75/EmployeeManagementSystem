using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    public class LeaveController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public LeaveController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Employee: View My Leave Requests
        [HttpGet]
        [Authorize (Roles ="Employee")]
        public async Task<IActionResult> MyLeaves()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var leaves = await _context.LeaveRequests
                .Where(l => l.EmployeeId == user.Id)
                .OrderByDescending(l => l.RequestDate)
                .ToListAsync();

            return View(leaves);
        }


        // Employee: Apply for Leave
        [HttpGet]
        public IActionResult ApplyLeave()
        {
            return View(new LeaveRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyLeave(LeaveRequest model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Unauthorized();
                }

                // Assign employee automatically
                model.EmployeeId = user.Id;
                model.Status = "Pending";
                model.RequestDate = DateTime.Now;

                _context.LeaveRequests.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(MyLeaves));
            }

            return View(model);
        }


        // Admin: Manage All Leave Requests
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageLeaves()
        {
            var leaves = await _context.LeaveRequests
                .Include(l => l.Employee)   // ✅ must use navigation property
                .OrderByDescending(l => l.RequestDate)
                .ToListAsync();
            return View(leaves);
        }

        // Admin: Approve
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var leave = await _context.LeaveRequests.FindAsync(id);
            if (leave != null)
            {
                leave.Status = "Approved";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageLeaves");
        }

        // Admin: Reject
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var leave = await _context.LeaveRequests.FindAsync(id);
            if (leave != null)
            {
                leave.Status = "Rejected";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageLeaves");
        }
    }
}
