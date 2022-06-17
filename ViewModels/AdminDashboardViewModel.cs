using StudentManagementSystem.Models;
using System.Collections.Generic;

namespace StudentManagementSystem.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int AdminsCount { get; set; }
        public int TeachersCount { get; set; }
        public int StudentsCount { get; set; }
        public List<Course> Courses { get; set; }
        public int RoomsCount { get; set; }
    }
}
