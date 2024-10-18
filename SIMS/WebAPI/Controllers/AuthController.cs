using Azure.Core;
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
        private readonly UserRepository _userRepository;

        public AuthController(JwtService jwtservice, RedisTokenStore redisTokenStore, UserRepository userRepository, AuthRepository authRepository)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            try
            {
                object tokens = await _authRepository.Login(login.Email, login.Password);
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
            try
            {
                if (refresh_token.Token != "")
                {
                    object tokens = await _authRepository.Refresh(refresh_token.Token);
                    return Ok(tokens);
                }
                else
                    return BadRequest("No Refresh Token provided");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
