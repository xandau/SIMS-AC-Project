using WebAPI.Models;

namespace WebAPI.Repository
{
    public class UserRepository : ARepository<User>
    {
        public UserRepository(SIMSContext context) : base(context)
        {

        }

        public async Task<User> GetUserByMailAsync(string mail, string password)
        {
            throw new NotImplementedException();
        }
    }
}
