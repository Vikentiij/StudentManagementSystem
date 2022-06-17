using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Areas.Identity.Data;
using StudentManagementSystem.Data;
using StudentManagementSystem.ViewModels;
using System.Linq;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminDashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var teachersCount = _context.Teachers.Count();
            var studentsCount = _context.Students.Count();

            // To not deal with Identity Framework roles, get the adming count by sybsctracting teachers and students from total user count
            var adminsCount = _userManager.Users.Count() - teachersCount - studentsCount;

            var courses = _context.Courses.Include(c => c.Students).ToList();
            foreach (var course in courses)
            {
                var students = _context.StudentCourse.Where(s => s.CourseId == course.CourseId).Select(s => s.Student).ToList();
                course.Students = students;
            }

            var roomsCount = _context.Room.Count();

            var model = new AdminDashboardViewModel()
            {
                AdminsCount = adminsCount,
                TeachersCount = teachersCount,
                StudentsCount = studentsCount,
                RoomsCount = roomsCount,
                Courses = courses,
            };
            
            return View(model);
        }
    }
}
