using mera_school.Data;
using mera_school.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace mera_school.Controllers
{
    public class SubjectController : Controller
    {
        private readonly SchoolDbContext _context;

        public SubjectController(SchoolDbContext context)
        {
            _context = context;
        }

        // GET: Subject
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            var subjects = _context.Subjects.Include(s => s.Teacher).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                subjects = subjects.Where(s => s.SubjectName.Contains(searchString) || s.SubjectCode.Contains(searchString));
            }

            int pageSize = 10;
            int pageIndex = pageNumber ?? 1;
            int totalRecords = await subjects.CountAsync();
            var items = await subjects.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentPage = pageIndex;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return View(items);
        }

        // GET: Subject/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var subject = await _context.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(m => m.SubjectId == id);
            
            if (subject == null) return NotFound();

            return View(subject);
        }

        // GET: Subject/Create
        public IActionResult Create()
        {
            ViewBag.TeacherId = new SelectList(_context.Teachers, "Id", "FullName");
            return View();
        }

        // POST: Subject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subject subject)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subject);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Subject created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TeacherId = new SelectList(_context.Teachers, "Id", "FullName", subject.TeacherId);
            return View(subject);
        }

        // GET: Subject/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();

            ViewBag.TeacherId = new SelectList(_context.Teachers, "Id", "FullName", subject.TeacherId);
            return View(subject);
        }

        // POST: Subject/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Subject subject)
        {
            if (id != subject.SubjectId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Subject updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(subject.SubjectId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TeacherId = new SelectList(_context.Teachers, "Id", "FullName", subject.TeacherId);
            return View(subject);
        }

        // POST: Subject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Subject deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.SubjectId == id);
        }
    }
}
