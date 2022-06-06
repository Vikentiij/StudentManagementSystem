using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        public int TeacherId { get; set; }

        [Display(Name = "Course Name")]
        public string Name { get; set; }
        
        public string Notes { get; set; }


        public virtual Teacher Teacher { get; set; }
        public virtual List<Student> Students { get; set; }
    }
}
