using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using RTools_NTS.Util;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebAPI.Models;

namespace WebAPI.AuthServices
{
    public class JwtService
    {
        private readonly string? secretKey;
        private readonly SymmetricSecurityKey key;
        private readonly SigningCredentials credentials;

        public JwtService()
        {
#if DEBUG
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            secretKey = config["JWTSettings-Secret"];
#else
            IConfigurationRoot config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            secretKey = config["JWTSettings-Secret"];
#endif
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public long GetClaimsFromToken(string token)
        {
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            try
            {
                ClaimsPrincipal claimsPrincipal = handler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                return Convert.ToInt64(claimsPrincipal.Claims.FirstOrDefault().Value);
            }
            catch 
            {
                return 0;
            }
        }
    }
}
