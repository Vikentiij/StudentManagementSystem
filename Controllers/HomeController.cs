using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentManagementSystem.Models;
using StudentManagementSystem.Utils;
using System;
using System.Diagnostics;

namespace StudentManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (this.User.IsInRole("Admin")) return RedirectToAction("Index", "AdminDashboard");
            else if (this.User.IsInRole("Teacher")) return RedirectToAction("Index", "TeacherDashboard");
            else if (this.User.IsInRole("Student")) return RedirectToAction("Index", "StudentDashboard");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ContactUs(SendMailDto sendMailDto)
        {
            if (!ModelState.IsValid) return View();

            try
            {
                var body = $"Name : {sendMailDto.Name} <br/> Message : {sendMailDto.Message}";

                Email.Send("testmvcemailproject@gmail.com", sendMailDto.Subject, body);

                ViewBag.Message = "Mail Send";

                // now i need to create the from 
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                //If any error occured it will show
                ViewBag.Message = ex.Message.ToString();
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
