using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class SendMailDto
    {
        public string RecipientName { get; set; }

        public string RecipientEmail { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
