using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class Assesment
    {
        public int AssesmentId { get; set; }

        public int CourseId { get; set; }
        
        public string Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }


        public virtual Course Course { get; set; }
        public virtual List<Student> Students { get; set; }
    }
}
