using System.Threading.Tasks;

namespace PruebaAngular.Infrastructure.Messaging
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
