namespace StudentManagementSystem.Services
{
    public class AuthMessageSenderOptions
    {
        public string? FromEmail { get; set; }
        public string? SmtpPassword { get; set; }
        public string? SmtpServer { get; set; }
    }
}
