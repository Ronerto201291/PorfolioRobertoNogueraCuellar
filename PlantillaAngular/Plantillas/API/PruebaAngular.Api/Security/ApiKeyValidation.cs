using System.Threading.Tasks;

namespace PruebaAngular.Api.Security
{
    public interface IApiKeyValidation
    {
        Task<bool> IsValidApiKey(string apiKey);
    }

    public class ApiKeyValidation : IApiKeyValidation
    {
        public Task<bool> IsValidApiKey(string apiKey)
        {
            // Placeholder implementation
            return Task.FromResult(!string.IsNullOrEmpty(apiKey));
        }
    }
}
