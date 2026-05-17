using mera_school.Data;
using mera_school.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mera_school.Controllers
{
    public class TeacherController : Controller
    {
        private readonly SchoolDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TeacherController(SchoolDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Teacher/Index
        public async Task<IActionResult> Index(string search = "", string sortBy = "name",
                                               string subject = "", int pageNum = 1)
        {
            const int pageSize = 8;

            var query = _context.Teachers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(t => t.FullName.Contains(search) ||
                                         t.Email.Contains(search) ||
                                         t.Subject.Contains(search));

            if (!string.IsNullOrWhiteSpace(subject))
                query = query.Where(t => t.Subject == subject);

            query = sortBy switch
            {
                "salary"  => query.OrderByDescending(t => t.Salary),
                "subject" => query.OrderBy(t => t.Subject),
                "date"    => query.OrderByDescending(t => t.JoinDate),
                _         => query.OrderBy(t => t.FullName)
            };

            int totalRecords = await query.CountAsync();
            var teachers = await query.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Search       = search;
            ViewBag.SortBy       = sortBy;
            ViewBag.Subject      = subject;
            ViewBag.Page         = pageNum;
            ViewBag.TotalPages   = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.TotalRecords = totalRecords;
            ViewBag.Subjects     = await _context.Teachers.Select(t => t.Subject).Distinct().ToListAsync();

            return View(teachers);
        }

        // GET: Teacher/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Attendances)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null) return NotFound();
            return View(teacher);
        }

        // GET: Teacher/Create
        public IActionResult Create() => View();

        // POST: Teacher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                teacher.TeacherImage = await SaveImageAsync(imageFile, "teachers");
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Teacher added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: Teacher/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        // POST: Teacher/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Teacher teacher, IFormFile? imageFile)
        {
            if (id != teacher.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Teachers.FindAsync(id);
                    if (existing == null) return NotFound();

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        DeleteImage(existing.TeacherImage, "teachers");
                        existing.TeacherImage = await SaveImageAsync(imageFile, "teachers");
                    }

                    existing.FullName      = teacher.FullName;
                    existing.Subject       = teacher.Subject;
                    existing.Qualification = teacher.Qualification;
                    existing.Email         = teacher.Email;
                    existing.PhoneNumber   = teacher.PhoneNumber;
                    existing.Salary        = teacher.Salary;
                    existing.JoinDate      = teacher.JoinDate;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Teacher updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Teachers.Any(t => t.Id == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // POST: Teacher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                DeleteImage(teacher.TeacherImage, "teachers");
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Teacher deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private async Task<string?> SaveImageAsync(IFormFile? file, string folder)
        {
            if (file == null || file.Length == 0) return null;

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{folder}/{fileName}";
        }

        private void DeleteImage(string? imagePath, string folder)
        {
            if (string.IsNullOrEmpty(imagePath)) return;
            var fullPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}
