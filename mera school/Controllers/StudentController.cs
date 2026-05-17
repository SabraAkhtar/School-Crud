using mera_school.Data;
using mera_school.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mera_school.Controllers
{
    public class StudentController : Controller
    {
        private readonly SchoolDbContext _context;
        private readonly IWebHostEnvironment _env;

        public StudentController(SchoolDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Student/Index
        public async Task<IActionResult> Index(string search = "", string sortBy = "name",
                                               string gender = "", string className = "",
                                               int pageNum = 1)
        {
            const int pageSize = 8;

            var query = _context.Students.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(s => s.FullName.Contains(search) ||
                                         s.Email.Contains(search) ||
                                         s.ClassName.Contains(search));

            // Filter by gender
            if (!string.IsNullOrWhiteSpace(gender))
                query = query.Where(s => s.Gender == gender);

            // Filter by class
            if (!string.IsNullOrWhiteSpace(className))
                query = query.Where(s => s.ClassName == className);

            // Sort
            query = sortBy switch
            {
                "age"   => query.OrderBy(s => s.Age),
                "class" => query.OrderBy(s => s.ClassName),
                "date"  => query.OrderByDescending(s => s.AdmissionDate),
                _       => query.OrderBy(s => s.FullName)
            };

            int totalRecords = await query.CountAsync();
            var students = await query.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Search    = search;
            ViewBag.SortBy    = sortBy;
            ViewBag.Gender    = gender;
            ViewBag.ClassName = className;
            ViewBag.Page      = pageNum;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.TotalRecords = totalRecords;
            ViewBag.Classes   = await _context.Students.Select(s => s.ClassName).Distinct().ToListAsync();

            return View(students);
        }

        // GET: Student/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var student = await _context.Students
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();
            return View(student);
        }

        // GET: Student/Create
        public IActionResult Create() => View();

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                student.StudentImage = await SaveImageAsync(imageFile, "students");
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Student added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Student/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student, IFormFile? imageFile)
        {
            if (id != student.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Students.FindAsync(id);
                    if (existing == null) return NotFound();

                    // Update image only if a new one is uploaded
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        DeleteImage(existing.StudentImage, "students");
                        existing.StudentImage = await SaveImageAsync(imageFile, "students");
                    }

                    existing.FullName      = student.FullName;
                    existing.Gender        = student.Gender;
                    existing.Age           = student.Age;
                    existing.Email         = student.Email;
                    existing.PhoneNumber   = student.PhoneNumber;
                    existing.Address       = student.Address;
                    existing.ClassName     = student.ClassName;
                    existing.AdmissionDate = student.AdmissionDate;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Student updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Students.Any(s => s.Id == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                DeleteImage(student.StudentImage, "students");
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Student deleted successfully!";
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
