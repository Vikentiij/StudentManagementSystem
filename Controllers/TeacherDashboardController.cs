using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Data;
using StudentManagementSystem.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeacherDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var IFUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var teacher = _context.Teachers.FirstOrDefault(t => t.UserData.Id == IFUserId);

            var teacherCourseIds = new List<int>();
            if (teacher != null) teacherCourseIds = _context.Courses.Where(c => c.Teacher.Id == teacher.Id).Select(c => c.CourseId).ToList();

            var studentsCount = _context.StudentCourse.Where(s => teacherCourseIds.Contains(s.CourseId)).Select(s => s.Id).Distinct().Count();
            var roomsCount = _context.Room.Count();

            var model = new TeacherDashboardViewModel()
            {
                StudentsCount = studentsCount,
                CoursesCount = teacherCourseIds.Count(),
                RoomsCount = roomsCount,
            };

            return View(model);
        }
    }
}
