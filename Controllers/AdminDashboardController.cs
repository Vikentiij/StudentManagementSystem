using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Data;
using StudentManagementSystem.ViewModels;
using System.Linq;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            //var adminsCount = _context.Users.Where(u => u.Role == Models.Role.Admin).Count();
            var teachersCount = _context.Teachers.Count();
            var studentsCount = _context.Students.Count();
            var coursesCount = _context.Courses.Count();
            var roomsCount = _context.Room.Count();

            var model = new AdminDashboardViewModel()
            {
                AdminsCount = 1,
                TeachersCount = teachersCount,
                StudentsCount = studentsCount,
                CoursesCount = coursesCount,
                RoomsCount = roomsCount,
            };
            
            return View(model);
        }
    }
}
