using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Areas.Identity.Data;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Utils;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        private async Task<Role> GetUserRole(ApplicationUser user)
        {
            // We assume a user only has one role, so pick the highest one
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains("Admin")) return Role.Admin;
            else if (userRoles.Contains("Teacher")) return Role.Teacher;
            else return Role.Student;
        }

        // GET: Users
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> Index()
        {
            var users = new List<ApplicationUser>();

            if (User.IsInRole("Teacher") && !User.IsInRole("Admin"))
            {
                var IFUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var teacher = _context.Teachers.FirstOrDefault(t => t.UserData.Id == IFUserId);
                if (teacher != null)
                {
                    var teacherCourseIds = _context.Courses.Where(c => c.Teacher.Id == teacher.Id).Select(c => c.CourseId).ToList();
                    users = await _context.StudentCourse.Where(s => teacherCourseIds.Contains(s.CourseId)).Select(s => s.Student.UserData).Distinct().ToListAsync();
                }
            }

            if (User.IsInRole("Student") && !User.IsInRole("Teacher") &&  !User.IsInRole("Admin"))
            {
                var IFUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var student = _context.Students.FirstOrDefault(t => t.UserData.Id == IFUserId);

                if (student != null)
                {
                    var studentCourseIds = _context.StudentCourse.Where(c => c.Student.Id == student.Id).Select(c => c.CourseId).ToList();
                    users = await _context.StudentCourse.Where(s => studentCourseIds.Contains(s.CourseId)).Select(s => s.Student.UserData).Distinct().ToListAsync();
                }

               // var teachers = _context.Teachers.ToListAsync();
                // var admins = _context.Users.Where(c=>c.UserRoles.Id == Role.Admin).ToListAsync();
               // users = users include teachers;
            }

            else
            {
                users = await _userManager.Users.ToListAsync();
            }

            var userViewModel = new List<UserViewModel>();
            foreach (ApplicationUser user in users)
            {
                var thisViewModel = new UserViewModel();
                thisViewModel.UserId = user.Id;
                thisViewModel.Email = user.Email;
                thisViewModel.FirstName = user.FirstName;
                thisViewModel.LastName = user.LastName;
                thisViewModel.Role = await GetUserRole(user);
                userViewModel.Add(thisViewModel);
            }
            return View(userViewModel);
        }

        // GET: Users/Details/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == "")
            {
                return NotFound();
            }

            var user = await _userManager.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel()
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = await GetUserRole(user)
            };

            return View(userViewModel);
        }

        // GET: Users/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel user)
        {
            var newUser = new ApplicationUser
            {
                UserName = user.Email,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            string role = user.Role.ToString();
            string unencryptedPassword = user.Password;

            if (ModelState.IsValid)
            {
                // Creating IdentityFramework user
                await _userManager.CreateAsync(newUser, unencryptedPassword);
                await _userManager.AddToRoleAsync(newUser, role);

                // Creating Teacher or Student
                if (user.Role == Role.Teacher)
                {
                    await _context.Teachers.AddAsync(new Teacher { UserData = newUser });
                    await _context.SaveChangesAsync();
                }
                else if (user.Role == Role.Student)
                {
                    await _context.Students.AddAsync(new Student { UserData = newUser });
                    await _context.SaveChangesAsync();
                }

                // Sending email to new user
                var subject = "Account Registration";
                var body = $"<h3>Welcome to Student Management System, {user.FirstName}!</h3>\n" +
                $"<p>{role} account has been created for you</p>" +
                $"<p>Your temporary password is <code>{unencryptedPassword}</code> please change it when you first log in!</p>";
                Email.Send(user.Email, subject, body);

                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // GET: Users/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == "")
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel()
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = await GetUserRole(user)
            };

            return View(userViewModel);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserViewModel newUserData)
        {
            if (id != newUserData.UserId)
            {
                return NotFound();
            }

            var userToUpdate = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                userToUpdate.Email = newUserData.Email;
                userToUpdate.UserName = newUserData.Email;
                userToUpdate.FirstName = newUserData.FirstName;
                userToUpdate.LastName = newUserData.LastName;

                await _userManager.UpdateAsync(userToUpdate);

                return RedirectToAction(nameof(Index));
            }

            return View(newUserData);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == "")
            {
                return NotFound();
            }

            var user = await _userManager.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel()
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = await GetUserRole(user)
            };

            return View(userViewModel);
        }

        // POST: Users/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _userManager.Users.Any(e => e.Id == id);
        }
    }
}
