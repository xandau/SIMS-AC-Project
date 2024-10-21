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
            if (user.Blocked == true)
                throw new Exception("Cannot login: Blocked");

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            await _redisTokenStore.StoreRefreshTokenAsync(user.ID, user.UserName, refreshToken);

            return new { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        public async Task<object> Refresh(string refresh_token)
        {
            var entry = await _redisTokenStore.GetUserFromRefreshToken(refresh_token);
            
            if (entry.Length != 2)
                throw new Exception("Invalid refresh token");

            User? user = await _userRepository.GetUserByUsernameAsync(entry[1].Value.ToString());

            if (user.Blocked == true)
                throw new Exception("Cannot login: Blocked");

            var accessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            if (_redisTokenStore.RemoveRefreshTokenAsync(refresh_token).Result == true)
                _redisTokenStore?.StoreRefreshTokenAsync(user.ID, user.UserName, newRefreshToken);

            return new { AccessToken = accessToken, RefreshToken = newRefreshToken };
        }
    }
}
