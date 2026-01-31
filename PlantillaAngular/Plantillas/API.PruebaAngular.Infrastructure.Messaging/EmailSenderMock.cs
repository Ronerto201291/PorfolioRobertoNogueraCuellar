using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PruebaAngular.Infrastructure.Messaging
{
    // Simple mock email sender for development. Swap with real provider in production.
    public class EmailSenderMock : IEmailSender
    {
        private readonly ILogger<EmailSenderMock> _logger;

        public EmailSenderMock(ILogger<EmailSenderMock> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            _logger.LogInformation("[EmailSenderMock] To={To} Subject={Subject} Body={Body}", to, subject, body);
            return Task.CompletedTask;
        }
    }
}
