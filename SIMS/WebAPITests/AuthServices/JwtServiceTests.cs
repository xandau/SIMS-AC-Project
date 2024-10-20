using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPI.AuthServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace WebAPI.AuthServices.Tests
{
    // Debug Mode must be used
    [TestClass()]
    public class JwtServiceTests
    {
        [TestMethod()]
        public void GetClaimsFromTokenTest()
        {
            JwtService jwtService = new JwtService();
            User user = new User()
            {
                ID = 111,
                Email = "test@test.at"
            };

            string token = jwtService.GenerateAccessToken(user);
            long id = jwtService.GetClaimsFromToken(token);

            Assert.IsTrue(id > 0);
        }

        [TestMethod()]
        public void GenerateAccessTokenTest()
        {
            User user = new User()
            {
                ID = 111,
                Email = "test@test.at"
            };

            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            string? secretKey = config["JWTSettings-Secret"];

            JwtService jwtService = new JwtService();

            string token = jwtService.GenerateAccessToken(user);

            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            ClaimsPrincipal claimsPrincipal = handler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

            Assert.IsNotNull(claimsPrincipal);
        }
    }
}