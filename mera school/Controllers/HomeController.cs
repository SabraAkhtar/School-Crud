using mera_school.Data;
using mera_school.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace mera_school.Controllers
{
    public class HomeController : Controller
    {
        private readonly SchoolDbContext _context;

        public HomeController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Dashboard – aggregates key statistics for the admin panel.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            ViewBag.TotalStudents   = await _context.Students.CountAsync();
            ViewBag.TotalTeachers   = await _context.Teachers.CountAsync();
            ViewBag.TotalAttendance = await _context.Attendances.CountAsync();
            ViewBag.PresentToday    = await _context.Attendances
                                        .CountAsync(a => a.AttendanceDate == today && a.Status == "Present");
            ViewBag.AbsentToday     = await _context.Attendances
                                        .CountAsync(a => a.AttendanceDate == today && a.Status == "Absent");
            ViewBag.LeaveToday      = await _context.Attendances
                                        .CountAsync(a => a.AttendanceDate == today && a.Status == "Leave");

            // Recent attendance for dashboard table
            var recentAttendance = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Teacher)
                .OrderByDescending(a => a.AttendanceDate)
                .Take(5)
                .ToListAsync();

            return View(recentAttendance);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
