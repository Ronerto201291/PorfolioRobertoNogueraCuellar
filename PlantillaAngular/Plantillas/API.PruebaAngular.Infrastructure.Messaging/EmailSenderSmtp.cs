using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PruebaAngular.Infrastructure.Messaging
{
    // Simple SMTP implementation that reads settings from configuration.
    // Configuration path: Email:Smtp
    public class EmailSenderSmtp : IEmailSender
    {
        private readonly ILogger<EmailSenderSmtp> _logger;
        private readonly SmtpOptions _options;

        public EmailSenderSmtp(ILogger<EmailSenderSmtp> logger, IConfiguration configuration)
        {
            _logger = logger;
            _options = new SmtpOptions();
            configuration.GetSection("Email:Smtp").Bind(_options);
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(_options.Host))
            {
                _logger.LogWarning("SMTP host not configured, falling back to EmailSenderMock");
                return; // caller should log or fallback
            }

            try
            {
                using var client = new SmtpClient(_options.Host, _options.Port);
                client.EnableSsl = _options.EnableSsl;
                if (!string.IsNullOrWhiteSpace(_options.Username))
                {
                    client.Credentials = new NetworkCredential(_options.Username, _options.Password);
                }

                var mail = new MailMessage();
                mail.From = new MailAddress(_options.From ?? _options.Username ?? "no-reply@example.com");
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;

                await client.SendMailAsync(mail);
                _logger.LogInformation("Email sent to {To} via SMTP host {Host}", to, _options.Host);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}: {Error}", to, ex.Message);
                throw;
            }
        }

        private class SmtpOptions
        {
            public string Host { get; set; } = string.Empty;
            public int Port { get; set; } = 587;
            public bool EnableSsl { get; set; } = true;
            public string? Username { get; set; }
            public string? Password { get; set; }
            public string? From { get; set; }
        }
    }
}
