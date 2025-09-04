using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using EmployeeManagementSystem.Data;
namespace EmployeeManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context  )
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new
            {
                TotalEmployees = _context.Employees.Count(),
                TotalAttendance = _context.Attendances.Count(),
                TotalLeaves = _context.LeaveRequests.Count(),
                TotalTasks = _context.Tasks.Count()
            };
            return View(model);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
