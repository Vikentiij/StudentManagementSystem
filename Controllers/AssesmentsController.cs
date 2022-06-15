using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class AssesmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssesmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Assesments
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> Index()
        {
            var assesments = new List<Assesment>();
            var IFUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.IsInRole("Admin"))
            {
                assesments = await _context.Assesment.Include(a => a.Course).ToListAsync();
            }
            else if (User.IsInRole("Teacher"))
            {
                var teacherId = _context.Teachers.Include(t => t.UserData).FirstOrDefault(t => t.UserData.Id == IFUserId).Id;
                var courseIds = _context.Courses.Include(c => c.Teacher).Where(c => c.TeacherId == teacherId).Select(c => c.CourseId).ToList();
                assesments = await _context.Assesment.Include(a => a.Course).Where(a => courseIds.Contains(a.CourseId)).ToListAsync();
            }
            else if (User.IsInRole("Student"))
            {
                var studentId = _context.Students.Include(s => s.UserData).FirstOrDefault(s => s.UserData.Id == IFUserId).Id;
                var AssesmentIds = await _context.StudentAssesment.Where(c => c.Student.Id == studentId).Select(c => c.AssesmentId).ToListAsync();
                assesments = await _context.Assesment.Include(a => a.Course).Where(a => AssesmentIds.Contains(a.AssesmentId)).ToListAsync();
            }

           return View(assesments);
        }

        // GET: Assesments/Details/5
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assesment = await _context.Assesment
                .Include(a => a.Course)
                .FirstOrDefaultAsync(m => m.AssesmentId == id);
            if (assesment == null)
            {
                return NotFound();
            }

            return View(assesment);
        }

        // GET: Assesments/Create
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Create()
        {
            var courses = new List<Course>();

            if (User.IsInRole("Admin"))
            {
                courses = _context.Courses.ToList();
            }
            else
            {
                var IFUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var teacherId = _context.Teachers.Include(t => t.UserData).FirstOrDefault(t => t.UserData.Id == IFUserId).Id;
                courses = _context.Courses.Include(c => c.Teacher).Where(c => c.TeacherId == teacherId).ToList();
            }

            ViewData["CourseId"] = new SelectList(courses, "CourseId", "Name");
            return View();
        }

        // POST: Assesments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Create([Bind("AssesmentId,CourseId,Title,DueDate")] Assesment assesment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assesment);
                await _context.SaveChangesAsync();

                var studentIds = _context.StudentCourse.Include(s => s.Student).Where(c => c.CourseId == assesment.CourseId).Select(s => s.StudentId).ToList();
                foreach (var studentId in studentIds)
                {
                    var studentAssesment = new StudentAssesment()
                    {
                        AssesmentId = assesment.AssesmentId,
                        StudentId = studentId
                    };
                    _context.Add(studentAssesment);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Name", assesment.CourseId);
            return View(assesment);
        }

        // GET: Assesments/Edit/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assesment = await _context.Assesment.FindAsync(id);
            if (assesment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Name", assesment.CourseId);
            return View(assesment);
        }

        // POST: Assesments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Edit(int id, [Bind("AssesmentId,CourseId,Title,DueDate")] Assesment assesment)
        {
            if (id != assesment.AssesmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assesment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssesmentExists(assesment.AssesmentId))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Name", assesment.CourseId);
            return View(assesment);
        }

        // GET: Assesments/Delete/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assesment = await _context.Assesment
                .Include(a => a.Course)
                .FirstOrDefaultAsync(m => m.AssesmentId == id);
            if (assesment == null)
            {
                return NotFound();
            }

            return View(assesment);
        }

        // POST: Assesments/Delete/5
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assesment = await _context.Assesment.FindAsync(id);
            _context.Assesment.Remove(assesment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssesmentExists(int id)
        {
            return _context.Assesment.Any(e => e.AssesmentId == id);
        }
    }
}
