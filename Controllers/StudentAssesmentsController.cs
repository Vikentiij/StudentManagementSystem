using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class StudentAssesmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentAssesmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StudentAssesments
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StudentAssesment.Include(s => s.Assesment).Include(s => s.Student.UserData);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StudentAssesments/Details/5
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentAssesment = await _context.StudentAssesment
                .Include(s => s.Assesment)
                .Include(s => s.Student.UserData)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentAssesment == null)
            {
                return NotFound();
            }

            return View(studentAssesment);
        }

        // GET: StudentAssesments/Create
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Create()
        {
            ViewData["AssesmentId"] = new SelectList(_context.Assesment, "AssesmentId", "Title");
            ViewData["StudentId"] = new SelectList(_context.Students.Include(s => s.UserData), "Id", "FullName");
            return View();
        }

        // POST: StudentAssesments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,AssesmentId,Mark")] StudentAssesment studentAssesment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentAssesment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssesmentId"] = new SelectList(_context.Assesment, "AssesmentId", "Title", studentAssesment.AssesmentId);
            ViewData["StudentId"] = new SelectList(_context.Students.Include(s => s.UserData), "Id", "FullName", studentAssesment.StudentId);
            return View(studentAssesment);
        }

        // GET: StudentAssesments/Edit/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentAssesment = await _context.StudentAssesment.FindAsync(id);
            if (studentAssesment == null)
            {
                return NotFound();
            }
            ViewData["AssesmentId"] = new SelectList(_context.Assesment, "AssesmentId", "Title", studentAssesment.AssesmentId);
            ViewData["StudentId"] = new SelectList(_context.Students.Include(s => s.UserData), "Id", "FullName", studentAssesment.StudentId);
            return View(studentAssesment);
        }

        // POST: StudentAssesments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,AssesmentId,Mark")] StudentAssesment studentAssesment)
        {
            if (id != studentAssesment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentAssesment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentAssesmentExists(studentAssesment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssesmentId"] = new SelectList(_context.Assesment, "AssesmentId", "Title", studentAssesment.AssesmentId);
            ViewData["StudentId"] = new SelectList(_context.Students.Include(s => s.UserData), "Id", "FullName", studentAssesment.StudentId);
            return View(studentAssesment);
        }

        // GET: StudentAssesments/Delete/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentAssesment = await _context.StudentAssesment
                .Include(s => s.Assesment)
                .Include(s => s.Student.UserData)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentAssesment == null)
            {
                return NotFound();
            }

            return View(studentAssesment);
        }

        // POST: StudentAssesments/Delete/5
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentAssesment = await _context.StudentAssesment.FindAsync(id);
            _context.StudentAssesment.Remove(studentAssesment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentAssesmentExists(int id)
        {
            return _context.StudentAssesment.Any(e => e.Id == id);
        }
    }
}
