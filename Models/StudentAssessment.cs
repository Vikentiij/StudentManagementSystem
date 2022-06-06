namespace StudentManagementSystem.Models
{
    public class StudentAssesment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public int AssesmentId { get; set; }
        public byte Mark { get; set; }

        public virtual Student Student { get; set; }
        public virtual Assesment Assesment { get; set; }
    }
}
