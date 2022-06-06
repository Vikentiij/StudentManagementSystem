using StudentManagementSystem.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.Models
{
    public class Teacher
    { 
        public int Id { get; set; }

        public virtual ApplicationUser UserData { get; set; }

        [NotMapped]
        public string FirstName
        {
            get { return UserData.FirstName; }
        }

        [NotMapped]
        public string LastName
        {
            get { return UserData.LastName; }
        }

        [NotMapped]
        public string FullName
        {
            get { return $"{UserData.FirstName} {UserData.LastName}"; }
        }

        [NotMapped]
        public string Email
        {
            get { return UserData.Email; }
        }
    }
}
