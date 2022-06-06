using StudentManagementSystem.Areas.Identity.Data;

namespace StudentManagementSystem.Models
{
    public class Student
    {
        public int Id { get; set; }

        public virtual ApplicationUser UserData { get; set; }
    }
}
