using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Repository
{
    public class UserRepository : ARepository<User>
    {
        public UserRepository(SIMSContext context) : base(context)
        {

        }

        public async Task<User?> GetUserByMailAsync(string mail, string password)
        {
            return await _entities.SingleOrDefaultAsync(u => u.Email == mail && u.Password == password);
        }

        internal async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _entities.SingleOrDefaultAsync(u => u.UserName == username);
        }
    }
}
