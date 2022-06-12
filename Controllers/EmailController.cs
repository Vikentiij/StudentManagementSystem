using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Areas.Identity.Data;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagementSystem.Controllers
{
    public class EmailController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public EmailController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender
            )
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: EmailUser/5
          [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> EmailUser(string id)
        {
            if (id == "")
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var fromUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fromUser = await _userManager.Users.FirstOrDefaultAsync(m => m.Id == fromUserId);

            var model = new SendMailDto()
            {
                RecipientName = user.FullName,
                RecipientEmail = user.Email,
                Name = fromUser.FullName,
                Subject = "",
                Message = "",
            };

            return View(model);
        }

        // POST: EmailUser/5
        [Authorize(Roles = "Admin,Teacher,Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailUserAsync(SendMailDto sendMail)
        {
            if (ModelState.IsValid)
            {
                // Send email to the user
                var subject = sendMail.Subject;
                var body = sendMail.Message;
                await _emailSender.SendEmailAsync(sendMail.RecipientEmail, subject, body);

                return RedirectToAction("Index", "Users");
            }

            return View(sendMail);
        }

        // GET: EmailCourse/5        

        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> EmailCourse(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseName = await _context.Courses.Where(c => c.CourseId == id).Select(c => c.Name).FirstOrDefaultAsync();
            if (courseName == null)
            {
                ViewBag.Message = "Course not found";
                return View("Error");
            }

            var studentEmails = await _context.StudentCourse.Include(s => s.Student.UserData).Where(m => m.CourseId == id).Select(s => s.Student.Email).ToListAsync();
            if (studentEmails.Count() == 0)
            {
                ViewBag.Message = "No students enrolled in the course";
                return View("Error");
            }

            var fromUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fromUser = await _userManager.Users.FirstOrDefaultAsync(m => m.Id == fromUserId);

            var model = new SendMailDto()
            {
                RecipientName = courseName,
                RecipientEmail = string.Join(", ", studentEmails),
                Name = fromUser.FullName,
                Subject = "",
                Message = "",
            };

            return View("EmailUser", model);
        }

        // POST: EmailCourse/5
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailCourseAsync(SendMailDto sendMail)
        {
            if (ModelState.IsValid)
            {
                // Send email to the user
                var subject = sendMail.Subject;
                var body = sendMail.Message;
                await _emailSender.SendEmailAsync(sendMail.RecipientEmail, subject, body);

                return RedirectToAction("Index", "Users");
            }

            return View(sendMail);
        }
    }
}
