using System;
using System.Runtime.Serialization;

namespace PikaStatus.Middlewares
{
    [DataContract]
    public class TokenResponse
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }

        [DataMember(Name = "refresh_token")]
        public string? RefreshToken { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; } = string.Empty;
    }
}
