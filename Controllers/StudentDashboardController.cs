using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Data;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var IFUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Students.FirstOrDefault(t => t.UserData.Id == IFUserId);

            var StudentCourseId = _context.StudentCourse.Where(c => c.Student.Id == user.Id).Select(c => c.CourseId).ToList();
            var AssesmentId = _context.StudentAssesment.Where(c => c.Student.Id == user.Id).Select(c => c.AssesmentId).ToList();
            var CourseCount = _context.StudentCourse.Where(s => StudentCourseId.Contains(s.Id)).Select(s => s.Student).Distinct().Count();
            

            var model = new StudentDashboardViewModel()
            {
                CourseCount = CourseCount,
                AssesmentCount = AssesmentId.Count()
            };


            return View(model);
        }
    }
}
