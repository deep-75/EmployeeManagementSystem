using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Data;

namespace EmployeeManagementSystem.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AttendanceController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Employee: Check In
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CheckIn()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var today = DateTime.Now.Date;

            // Check if attendance already exists
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == user.Id && a.Date == today);

            if (attendance != null && attendance.CheckInTime.HasValue)
            {
                TempData["Error"] = "You have already checked in today!";
                return RedirectToAction("MyAttendance");
            }

            if (attendance == null)
            {
                attendance = new Attendance
                {
                    EmployeeId = user.Id,
                    Date = today,
                    CheckInTime = DateTime.Now
                };
                _context.Attendances.Add(attendance);
            }
            else
            {
                attendance.CheckInTime = DateTime.Now;
                _context.Attendances.Update(attendance);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Checked in successfully!";
            return RedirectToAction("MyAttendance");
        }


        // Employee: Check Out
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CheckOut()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var today = DateTime.Now.Date;

            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == user.Id && a.Date == today);

            if (attendance == null || !attendance.CheckInTime.HasValue)
            {
                TempData["Error"] = "You need to check in before checking out!";
                return RedirectToAction("MyAttendance");
            }

            if (attendance.CheckOutTime.HasValue)
            {
                TempData["Error"] = "You have already checked out today!";
                return RedirectToAction("MyAttendance");
            }

            attendance.CheckOutTime = DateTime.Now;

            // Calculate full/half day
            var duration = attendance.CheckOutTime.Value - attendance.CheckInTime.Value;
            attendance.DayType = duration >= AttendanceRules.FullDayThreshold ? "Full Day" : "Half Day";

            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Checked out successfully! Day marked as {attendance.DayType}";

            return RedirectToAction("MyAttendance");
        }


        // View Attendance
        [Authorize(Roles ="Employee")]
        public async Task<IActionResult> MyAttendance()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null) return NotFound();

            var records = await _context.Attendances
                .Where(a => a.EmployeeId == user.Id)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
            return View(records);
        }

        //Admin View all attendance
        [Authorize (Roles ="Admin")]
        public async Task<IActionResult> AllAttendance()
        {
            var records = await _context.Attendances
                .Include(a => a.Employee)
                .OrderByDescending (a => a.Date)
                .ToListAsync();
            return View(records);
        }
        // GET: Attendance/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var attendance = await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null) return NotFound();

            return View(attendance);  // confirmation page
        }

        // POST: Attendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Attendance deleted successfully!";
            }

            return RedirectToAction(nameof(AllAttendance));
        }

    }
}
