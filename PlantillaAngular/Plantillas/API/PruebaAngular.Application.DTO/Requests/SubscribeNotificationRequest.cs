using System.ComponentModel.DataAnnotations;

namespace PruebaAngular.Application.DTO.Requests
{
    public class SubscribeNotificationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
