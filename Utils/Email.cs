using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace StudentManagementSystem.Utils
{
    public class Email
    {
        public static void Send(string to, string subject, string body)
        {
            if (!to.Contains("gmail"))
            {
                Debug.WriteLine($"Skipping non-existing email {to}");
                return;
            }

            var registrationEmail = new MailMessage()
            {
                From = new MailAddress("testmvcemailproject@gmail.com"),
                Subject = subject,
                IsBodyHtml = true,
                Body = body
            };
            registrationEmail.To.Add(to);

            //you need to pass mail server address and you can also specify the port number if you required
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

            //Create nerwork credential and you need to give from email address and password
            NetworkCredential networkCredential = new NetworkCredential("testmvcemailproject@gmail.com", "Mvcgroup123");
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = networkCredential;
            smtpClient.Port = 587; // this is default port number - you can also change this
            smtpClient.EnableSsl = true; // if ssl required you need to enable it
            smtpClient.Send(registrationEmail);
        }
    }
}
