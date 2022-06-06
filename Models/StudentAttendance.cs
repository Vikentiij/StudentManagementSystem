using System.Collections.Generic;

namespace StudentManagementSystem.Models
{
    public class StudentAttendance
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int StudentId { get; set; }
        public bool Attentded { get; set; }

        public virtual Timetable Event { get; set; }
        public virtual Student Student { get; set; }
    }
}
