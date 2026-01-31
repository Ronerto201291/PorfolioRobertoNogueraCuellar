using MediatR;

namespace PruebaAngular.Application.Commands
{
    public class SubscribeNotificationCommand : IRequest<SubscribeNotificationResult>
    {
        public string Email { get; set; } = string.Empty;
    }

    public class SubscribeNotificationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
