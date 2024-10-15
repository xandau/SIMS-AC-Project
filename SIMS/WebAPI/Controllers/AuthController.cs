using Microsoft.AspNetCore.Mvc;
using WebAPI.AuthServices;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("auth")]
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            User user = await _userRepository.GetUserByMailAsync(email, password);
            if (user == null)
                return Unauthorized("Inavlid Credentials");

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            await _redisTokenStore.StoreRefreshTokenAsync(user.UserID, user.UserName, refreshToken);

            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(string refresh_token)
        {
            var entry = await _redisTokenStore.GetUserFromRefreshToken(refresh_token);
            if (entry == null)
                return Unauthorized("Invalid refresh token");

            User user = await _userRepository.GetUserByUsernameAsync(entry[1].Value.ToString());

            var accessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            return Ok(entry);
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User user)
        {
            Console.WriteLine(user);
            return Ok(await _userRepository.CreateAsync(user));
        }
    }
}
