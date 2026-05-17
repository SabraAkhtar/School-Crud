using mera_school.Data;
using mera_school.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace mera_school.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly SchoolDbContext _context;

        public AttendanceController(SchoolDbContext context)
        {
            _context = context;
        }

        // GET: Attendance/Index
        public async Task<IActionResult> Index(string search = "", string status = "",
                                               string dateFilter = "", int pageNum = 1)
        {
            const int pageSize = 10;

            var query = _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Teacher)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(a => a.Student!.FullName.Contains(search) ||
                                          a.Teacher!.FullName.Contains(search));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status == status);

            if (!string.IsNullOrWhiteSpace(dateFilter) && DateTime.TryParse(dateFilter, out var date))
                query = query.Where(a => a.AttendanceDate == date);

            query = query.OrderByDescending(a => a.AttendanceDate);

            int totalRecords = await query.CountAsync();
            var records = await query.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Search       = search;
            ViewBag.Status       = status;
            ViewBag.DateFilter   = dateFilter;
            ViewBag.Page         = pageNum;
            ViewBag.TotalPages   = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.TotalRecords = totalRecords;

            return View(records);
        }

        // GET: Attendance/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var record = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Teacher)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (record == null) return NotFound();
            return View(record);
        }

        // GET: Attendance/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        // POST: Attendance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Attendance record added successfully!";
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdownsAsync(attendance.StudentId, attendance.TeacherId);
            return View(attendance);
        }

        // GET: Attendance/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var record = await _context.Attendances.FindAsync(id);
            if (record == null) return NotFound();
            await PopulateDropdownsAsync(record.StudentId, record.TeacherId);
            return View(record);
        }

        // POST: Attendance/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Attendance attendance)
        {
            if (id != attendance.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendance);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Attendance record updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Attendances.Any(a => a.Id == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdownsAsync(attendance.StudentId, attendance.TeacherId);
            return View(attendance);
        }

        // POST: Attendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var record = await _context.Attendances.FindAsync(id);
            if (record != null)
            {
                _context.Attendances.Remove(record);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Attendance record deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ── Helper ───────────────────────────────────────────────────────────

        private async Task PopulateDropdownsAsync(int studentId = 0, int teacherId = 0)
        {
            ViewBag.Students = new SelectList(
                await _context.Students.OrderBy(s => s.FullName).ToListAsync(),
                "Id", "FullName", studentId);

            ViewBag.Teachers = new SelectList(
                await _context.Teachers.OrderBy(t => t.FullName).ToListAsync(),
                "Id", "FullName", teacherId);
        }
    }
}
