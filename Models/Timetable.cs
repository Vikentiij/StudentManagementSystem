using System;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class Timetable
    {
        public int TimetableId { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime Day { get; set; }

        public int CourseId { get; set; }
        public int RoomId { get; set; }


        public virtual Course Course { get; set; }
        public virtual Room Room { get; set; }
    }
}
