using WebAPI.AuthServices;

namespace WebAPI.Repository
{
    public class AuthRepository
    {
        private readonly JwtService _jwtService;
        private readonly RedisTokenStore _redisTokenStore;
        private readonly UserRepository _userRepository;

        public AuthRepository(JwtService jwtservice, RedisTokenStore redisTokenStore, UserRepository userRepository)
        {
            _jwtService = jwtservice;
            _redisTokenStore = redisTokenStore;
            _userRepository = userRepository;
        }

        public async Task<object> Login(string email, string password)
        {
            User? user = await _userRepository.GetUserByMailAsync(email, password);
            if (user == null)
                throw new Exception("Invalid Credentials");
                //return Unauthorized("Invalid Credentials");

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            await _redisTokenStore.StoreRefreshTokenAsync(user.ID, user.UserName, refreshToken);

            return new { AccessToken = accessToken, RefreshToken = refreshToken };
        }
    }
}
