using Eticaret.Core.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Eticaret.Service
{
        public class EmailService : IEmailService
        {
            private readonly IConfiguration _configuration;

            public EmailService(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public async Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml = true)
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");

                using (SmtpClient client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"])))
                {
                    client.Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]);
                    client.EnableSsl = bool.Parse(smtpSettings["EnableSsl"]);

                    MailMessage message = new MailMessage
                    {
                        From = new MailAddress(smtpSettings["From"]),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = isBodyHtml
                    };

                    message.To.Add(to);

                    await client.SendMailAsync(message);
                }
            }
        }
    }


