using System.Security.Cryptography;
using System.Text;

namespace VideoConferenceApp.Services
{
    public class AgoraTokenService
    {
        private readonly string _appId;
        private readonly string _appCertificate;
        private readonly int _tokenExpirationInSeconds;

        public AgoraTokenService(IConfiguration configuration)
        {
            _appId = configuration["Agora:AppId"] ?? throw new ArgumentNullException("Agora:AppId");
            _appCertificate = configuration["Agora:AppCertificate"] ?? throw new ArgumentNullException("Agora:AppCertificate");
            _tokenExpirationInSeconds = int.Parse(configuration["Agora:TokenExpirationInSeconds"] ?? "3600");
        }

        public string GenerateToken(string channelName, string userId)
        {
            // This is a simplified token generation. In a real implementation, 
            // you would use the Agora SDK or a proper token builder.
            // For the sake of this example, we'll create a unique token by combining values 
            // and hashing them to simulate a token.
            
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var expireTime = timestamp + _tokenExpirationInSeconds;
            
            var tokenData = $"{_appId}:{channelName}:{userId}:{timestamp}:{expireTime}:{_appCertificate}";
            
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(tokenData));
            
            // Convert to Base64 and remove any characters that might be problematic in URLs
            return Convert.ToBase64String(hashBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }
    }
} 