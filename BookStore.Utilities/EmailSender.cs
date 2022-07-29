using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Utilities
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            

            var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse("hello@dotnetmastery.com"));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            //send email
            using (var emailClient = new MailKit.Net.Smtp.SmtpClient())                            // package - mailkit.net.smtp
            {
                emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate("avadhadiya@argusoft.com", "Argus@890");             // Don't forget to on less secure app from your google account
                emailClient.Send(emailToSend);
                emailClient.Disconnect(true);                                     // It is good practice to disconnect your email client after task completed
            }

            return Task.CompletedTask;
        }
    }
}
