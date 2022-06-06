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
    public class TimetablesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimetablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Timetables
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> Index()
        {
            var IFUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allTimetables = _context.Timetable.Include(t => t.Course).Include(t => t.Room).OrderBy(t => t.Day);  // Sorting by Day
            var timetables = new List<Timetable>();

            if (this.User.IsInRole("Admin"))
            {
                timetables = await allTimetables.ToListAsync();
            }
            else if (this.User.IsInRole("Teacher"))
            {
                var teacher = _context.Teachers.FirstOrDefault(t => t.UserData.Id == IFUserId);
                timetables = await allTimetables.Where(t => t.Course.TeacherId == teacher.Id).ToListAsync();
            }
            else if (this.User.IsInRole("Student"))
            {
                // TODO: filter by Student courses
                timetables = await allTimetables.ToListAsync();
            }

            return View(timetables);
        }

        // GET: Timetables/Details/5
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timetable = await _context.Timetable
                .Include(t => t.Course)
                .Include(t => t.Room)
                .FirstOrDefaultAsync(m => m.TimetableId == id);
            if (timetable == null)
            {
                return NotFound();
            }

            return View(timetable);
        }

        // GET: Timetables/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Name");
            ViewData["RoomId"] = new SelectList(_context.Room, "RoomId", "Name");
            return View();
        }

        // POST: Timetables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("TimetableId,Day,CourseId,RoomId")] Timetable timetable)
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Name", timetable.CourseId);
            ViewData["RoomId"] = new SelectList(_context.Room, "RoomId", "Name", timetable.RoomId);

            if (ModelState.IsValid)
            {
                var existingCourse = await _context.Timetable.Where(t => t.CourseId == timetable.CourseId && t.Day == timetable.Day).Include(t => t.Course).FirstOrDefaultAsync();
                if (existingCourse != null)
                {
                    ViewData["ErrorMessage"] = $"{existingCourse.Course.Name} timetable already exists for {timetable.Day:d}";
                    return View(timetable);
                }

                var occupiedRoom = await _context.Timetable.Where(t => t.RoomId == timetable.RoomId && t.Day == timetable.Day).Include(t => t.Room).FirstOrDefaultAsync();
                if (occupiedRoom != null)
                {
                    ViewData["ErrorMessage"] = $"Room {occupiedRoom.Room.Name} is occupied on {timetable.Day:d}";
                    return View(timetable);
                }

                _context.Add(timetable);
                await _context.SaveChangesAsync();

                // Creating corresponding attendance
                var enrolledStudents = await _context.StudentCourse.Where(s => s.CourseId == timetable.CourseId).ToListAsync();
                foreach (var student in enrolledStudents)
                {
                    var studentAttendance = new StudentAttendance()
                    {
                        Attentded = false,
                        EventId = timetable.TimetableId,
                        StudentId = student.Id                        
                    };
                    _context.Add(studentAttendance);
                    await _context.SaveChangesAsync();
                }
                
                return RedirectToAction(nameof(Index));
            }

            return View(timetable);
        }

        // GET: Timetables/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timetable = await _context.Timetable.FindAsync(id);
            if (timetable == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Name", timetable.CourseId);
            ViewData["RoomId"] = new SelectList(_context.Room, "RoomId", "Name", timetable.RoomId);
            return View(timetable);
        }

        // POST: Timetables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("TimetableId,Day,CourseId,RoomId")] Timetable timetable)
        {
            if (id != timetable.TimetableId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timetable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimetableExists(timetable.TimetableId))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Name", timetable.CourseId);
            ViewData["RoomId"] = new SelectList(_context.Room, "RoomId", "Name", timetable.RoomId);
            return View(timetable);
        }

        // GET: Timetables/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timetable = await _context.Timetable
                .Include(t => t.Course)
                .Include(t => t.Room)
                .FirstOrDefaultAsync(m => m.TimetableId == id);
            if (timetable == null)
            {
                return NotFound();
            }

            // Deleting corresponding attendance
            //_context.StudentAttendance.RemoveRange(_context.StudentAttendance.Where(s => s.EventId == timetable.TimetableId));
            //await _context.SaveChangesAsync();

            return View(timetable);
        }

        // POST: Timetables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timetable = await _context.Timetable.FindAsync(id);
            _context.Timetable.Remove(timetable);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimetableExists(int id)
        {
            return _context.Timetable.Any(e => e.TimetableId == id);
        }
    }
}
