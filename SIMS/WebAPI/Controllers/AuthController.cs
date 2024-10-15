using Microsoft.AspNetCore.Mvc;
using WebAPI.AuthServices;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly RedisTokenStore _redisTokenStore;
        private readonly UserRepository _userRepository;

        public AuthController(JwtService jwtservice, RedisTokenStore redisTokenStore, UserRepository userRepository)
        {
            _jwtService = jwtservice;
            _redisTokenStore = redisTokenStore;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Login(string email, string password)
        {
            User user = await _userRepository.GetUserByMailAsync(email, password);
            if (user == null)
            {
                return Unauthorized("Inavlid Credentials");
            };

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            await _redisTokenStore.StoreRefreshTokenAsync(user.UserID, user.UserName, refreshToken);

            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }
    }
}
