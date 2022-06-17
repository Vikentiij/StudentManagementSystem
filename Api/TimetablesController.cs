using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class TimetablesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TimetablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Timetables
        [HttpGet]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<ActionResult<IEnumerable<TimetableApiModel>>> GetTimetable()
        {
            var IFUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allTimetables = _context.Timetable.Include(t => t.Course);
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
                var studentId = _context.Students.FirstOrDefault(t => t.UserData.Id == IFUserId).Id;
                var studentCourse = _context.StudentCourse.FirstOrDefault(s => s.StudentId == studentId);
                timetables = await allTimetables.Where(t => t.Course.CourseId == studentCourse.CourseId).ToListAsync();
            }

            var timetableApi = new List<TimetableApiModel>();

            List<string> colors = new List<string>() { "red", "green", "blue", "teal", "wheat" };
            var courseColors = new Dictionary<string, string>();
            foreach (var item in timetables)
            {                
                if (!courseColors.ContainsKey(item.Course.Name)) {
                    if (courseColors.Keys.Count >= colors.Count) colors.AddRange(colors);
                    courseColors[item.Course.Name] = colors[courseColors.Keys.Count];                    
                }
                var color = courseColors[item.Course.Name];

                timetableApi.Add(new TimetableApiModel()
                {
                    start = item.Day.ToString("yyyy-MM-dd"),
                    title = item.Course.Name,
                    url = $"/Timetables/Details/{item.TimetableId}",
                    color = color
                });
            }

            return timetableApi;
        }

        // GET: api/Timetables/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Timetable>> GetTimetable(int id)
        {
            var timetable = await _context.Timetable.FindAsync(id);

            if (timetable == null)
            {
                return NotFound();
            }

            return timetable;
        }

        // PUT: api/Timetables/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTimetable(int id, Timetable timetable)
        {
            if (id != timetable.TimetableId)
            {
                return BadRequest();
            }

            _context.Entry(timetable).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimetableExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Timetables
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Timetable>> PostTimetable(Timetable timetable)
        {
            _context.Timetable.Add(timetable);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTimetable", new { id = timetable.TimetableId }, timetable);
        }

        // DELETE: api/Timetables/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimetable(int id)
        {
            var timetable = await _context.Timetable.FindAsync(id);
            if (timetable == null)
            {
                return NotFound();
            }

            _context.Timetable.Remove(timetable);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TimetableExists(int id)
        {
            return _context.Timetable.Any(e => e.TimetableId == id);
        }
    }
}
