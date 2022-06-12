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
    public class StudentCoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentCoursesController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: StudentCourses
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StudentCourse.Include(s => s.Course).Include(s => s.Student.UserData).OrderBy(s => s.Course.Name);
            ViewData["Courses"] = await _context.Courses.OrderBy(c => c.Name).ToListAsync();

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StudentCourses/Create/5
        [Authorize(Roles = "Admin")]
        public IActionResult Create(int? id)
        {
            var courseId = id;
            var course = _context.Courses.Where(c => c.CourseId == courseId);
            ViewData["CourseId"] = new SelectList(course, "CourseId", "Name");
            
            //var enrolledStudents = _context.StudentCourse.Where(c => c.CourseId == courseId).Select(s => s.StudentId).ToList();
            var enrolledStudents = _context.StudentCourse.Select(s => s.StudentId).ToList();
            ViewData["UserId"] = new SelectList(_context.Students.Include(s => s.UserData).Where(s => !enrolledStudents.Contains(s.Id)), "Id", "FullName");
            ViewData["UserId"] = new SelectList(_context.Students.Include(s => s.UserData).Where(s => !enrolledStudents.Contains(s.Id)), "Id", "FullName");

            ViewData["CourseName"] = course.FirstOrDefault().Name;

            return View();
        }

        // POST: StudentCourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,CourseId")] StudentCourse studentCourse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Name", studentCourse.CourseId);
            ViewData["UserId"] = new SelectList(_context.Students.Include(s => s.UserData), "StudentId", "FullName", studentCourse.StudentId);
            return View(studentCourse);
        }

        // GET: StudentCourses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentCourse = await _context.StudentCourse
                .Include(s => s.Course)
                .Include(s => s.Student.UserData)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentCourse == null)
            {
                return NotFound();
            }

            return View(studentCourse);
        }

        // POST: StudentCourses/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentCourse = await _context.StudentCourse.FindAsync(id);
            _context.StudentCourse.Remove(studentCourse);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentCourseExists(int id)
        {
            return _context.StudentCourse.Any(e => e.Id == id);
        }
    }
}
