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
    public class StudentAttendancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentAttendancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StudentAttendances
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Index()
        {
            var studentAssesments = new List<StudentAssesment>();
            var IFUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var timetableDays = new List<System.DateTime>();
            var courseList = new List<Course>();
            var studentAttendance = new List<StudentAttendance>();

            if (User.IsInRole("Admin"))
            {
                timetableDays = await _context.Timetable.Select(t => t.Day).OrderBy(t => t.Day).Distinct().ToListAsync();
                courseList = await _context.Courses.ToListAsync();
                studentAttendance = await _context.StudentAttendance.Include(s => s.Event).Include(s => s.Event.Course).Include(s => s.Student.UserData).ToListAsync();
            }
            else if (User.IsInRole("Teacher"))
            {
                var teacherId = _context.Teachers.Include(t => t.UserData).FirstOrDefault(t => t.UserData.Id == IFUserId).Id;
                var teacherCourses = _context.Courses.Include(c => c.Teacher).Where(c => c.TeacherId == teacherId);
                var courseIds = await teacherCourses.Select(c => c.CourseId).ToListAsync();

                courseList = await teacherCourses.ToListAsync();
                studentAttendance = await _context.StudentAttendance.Include(s => s.Event).Include(s => s.Event.Course).Include(s => s.Student.UserData).Where(s => courseIds.Contains(s.Event.CourseId)).ToListAsync();
                timetableDays = await _context.Timetable.Where(t => courseIds.Contains(t.CourseId)).Select(t => t.Day).OrderBy(t => t.Day).Distinct().ToListAsync();
            }

            ViewData["TimetableDays"] = timetableDays;
            ViewData["courseList"] = courseList;
            return View(studentAttendance);
        }

        // GET: StudentAttendances/Details/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentAttendance = await _context.StudentAttendance
                .Include(s => s.Event)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentAttendance == null)
            {
                return NotFound();
            }

            return View(studentAttendance);
        }

        // GET: StudentAttendances/Create
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Timetable.Include(t => t.Course), "TimetableId", "Course.Name");
            ViewData["StudentId"] = new SelectList(_context.Students.Include(s => s.UserData), "Id", "FullName");
            return View();
        }

        // POST: StudentAttendances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EventId,StudentId,Attentded")] StudentAttendance studentAttendance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentAttendance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Timetable, "TimetableId", "TimetableId", studentAttendance.EventId);
            ViewData["StudentId"] = new SelectList(_context.Students.Include(s => s.UserData), "UserId", "FullName", studentAttendance.StudentId);
            return View(studentAttendance);
        }

        // GET: StudentAttendances/Edit/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentAttendance = await _context.StudentAttendance.FindAsync(id);
            if (studentAttendance == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Timetable, "TimetableId", "TimetableId", studentAttendance.EventId);
            ViewData["StudentId"] = new SelectList(_context.Students.Include(s => s.UserData), "Id", "FullName", studentAttendance.StudentId);
            return View(studentAttendance);
        }

        // POST: StudentAttendances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EventId,StudentId,Attentded")] StudentAttendance studentAttendance)
        {
            if (id != studentAttendance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentAttendance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentAttendanceExists(studentAttendance.Id))
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
            ViewData["EventId"] = new SelectList(_context.Timetable, "TimetableId", "TimetableId", studentAttendance.EventId);
            ViewData["StudentId"] = new SelectList(_context.Students.Include(s => s.UserData), "Id", "FullName", studentAttendance.StudentId);
            return View(studentAttendance);
        }

        // GET: StudentAttendances/Delete/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentAttendance = await _context.StudentAttendance
                .Include(s => s.Event)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentAttendance == null)
            {
                return NotFound();
            }

            return View(studentAttendance);
        }

        // POST: StudentAttendances/Delete/5
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentAttendance = await _context.StudentAttendance.FindAsync(id);
            _context.StudentAttendance.Remove(studentAttendance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentAttendanceExists(int id)
        {
            return _context.StudentAttendance.Any(e => e.Id == id);
        }
    }
}
