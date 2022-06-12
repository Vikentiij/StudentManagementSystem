using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace StudentManagementSystem.Services
{
    // Microsoft documentation: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-5.0&tabs=visual-studio
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        public AuthMessageSenderOptions Options { get; }  // Set with Secret Manager.

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
        {
            Options = optionsAccessor.Value;
            _logger = logger;

            if (string.IsNullOrEmpty(Options.SmtpPassword))
            {
                throw new Exception("SmtpPassword is missing, set it in appsettings.json");
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if (!toEmail.Contains("@gmail.com") && !toEmail.Contains("@techtorium.ac.nz"))
            {
                _logger.LogInformation($"Skipping non-existing email {toEmail}");
                return;
            }

            var registrationEmail = new MailMessage()
            {
                From = new MailAddress(Options.FromEmail),
                Subject = subject,
                IsBodyHtml = true,
                Body = message
            };
            registrationEmail.To.Add(toEmail);

            //you need to pass mail server address and you can also specify the port number if you required
            SmtpClient smtpClient = new SmtpClient(Options.SmtpServer);

            //Create nerwork credential and you need to give from email address and password
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(Options.FromEmail, Options.SmtpPassword);
            smtpClient.Port = 587; // this is default port number - you can also change this
            smtpClient.EnableSsl = true; // if ssl required you need to enable it
            await smtpClient.SendMailAsync(registrationEmail);
        }

        public async Task ContactAdminAsync(string subject, string message)
        {
            // FromEmail is the SMTP server admin email
            await SendEmailAsync(Options.FromEmail, subject, message);
        }
    }
}
