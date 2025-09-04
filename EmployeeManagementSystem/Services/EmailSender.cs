using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // For now, log it instead of sending
            Console.WriteLine($"[EmailSender] To: {email}, Subject: {subject}, Body: {htmlMessage}");
            return Task.CompletedTask;
        }
    }
}
