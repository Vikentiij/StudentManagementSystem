namespace StudentManagementSystem.Models
{
    public class StudentAttendance
    {
        public int Id { get; set; }

        public Timetable Event { get; set; }
        public Student Student { get; set; }
        public bool Attentded { get; set; }
    }
}
