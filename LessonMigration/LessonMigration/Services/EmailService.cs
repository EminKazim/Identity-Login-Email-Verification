using LessonMigration.Data;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LessonMigration.Services
{
    public class EmailService:IEmailService
    {
        private readonly AppDbContext _context;
        public EmailService(AppDbContext context)
        {
            _context = context;
        }
        public async Task SendEmail(string emailaddress, string url)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("arastunsa@code.edu.az", "Fiorello");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress(emailaddress, "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = $"<a href={url}>Click here</a>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
