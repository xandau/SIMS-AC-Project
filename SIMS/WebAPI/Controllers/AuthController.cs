using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebAPI.AuthServices;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthRepository _authRepository;

        private readonly JwtService _jwtService;
        private readonly RedisTokenStore _redisTokenStore;
        private readonly UserRepository _userRepository;

        public AuthController(JwtService jwtservice, RedisTokenStore redisTokenStore, UserRepository userRepository, AuthRepository authRepository)
        {
            _jwtService = jwtservice;
            _redisTokenStore = redisTokenStore;
            _userRepository = userRepository;

            _authRepository = authRepository;
        }

        // TODO: Logik in Repo auslagern

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            try
            {
                object tokens = _authRepository.Login(login.Email, login.Password);
                return Ok(tokens);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDTO refresh_token)
        {
            var entry = await _redisTokenStore.GetUserFromRefreshToken(refresh_token.Token);
            if (entry.Length != 2)
                return Unauthorized("Invalid refresh token");

            User user = await _userRepository.GetUserByUsernameAsync(entry[1].Value.ToString());

            var accessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            if (_redisTokenStore.RemoveRefreshTokenAsync(refresh_token.Token).Result == true)
                _redisTokenStore?.StoreRefreshTokenAsync(user.ID, user.UserName, newRefreshToken);
            
            return Ok(new { AccessToken = accessToken, RefreshToken = newRefreshToken });
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User user)
        {
            try
            {
                await _userRepository.CreateAsync(user);
                return Ok();
            }
            catch (InvalidOperationException ex) when (ex.Message == "Email/User already exists.")
            {
                return Conflict(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message == "Email/User already exists.")
            {
                return Conflict(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
