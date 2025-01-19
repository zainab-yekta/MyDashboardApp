using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Threading.Tasks;


namespace MyDashboardApp.Services
{
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Simulate sending an email
            System.Diagnostics.Debug.WriteLine($"Sending email to: {email}");
            System.Diagnostics.Debug.WriteLine($"Subject: {subject}");
            System.Diagnostics.Debug.WriteLine($"Message: {htmlMessage}");
            return Task.CompletedTask;
        }
    }
    //Configure a Real Email Sender
    // public class SendGridEmailSender : IEmailSender
    //  {
    //     private readonly string _apiKey;

    //      public SendGridEmailSender(string apiKey)
    //      {
    //          _apiKey = apiKey;
    //      }

    // public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    //  {
    //      var client = new SendGrid.SendGridClient(_apiKey);
    //      var msg = new SendGrid.Helpers.Mail.SendGridMessage
    //      {
    //          From = new SendGrid.Helpers.Mail.EmailAddress("your_email@example.com", "Your Name"),
    //          Subject = subject,
    //          HtmlContent = htmlMessage
    //      };
    //      msg.AddTo(new SendGrid.Helpers.Mail.EmailAddress(email));
    //      await client.SendEmailAsync(msg);
    //  }
    // }

}
