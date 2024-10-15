using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using System;

namespace WebAPI.Repository
{
    public class UserRepository : ARepository<User>
    {
        public UserRepository(SIMSContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByMailAsync(string mail, string password)
        {
            return await _entities.SingleOrDefaultAsync(u => u.Email == mail && u.VerifyPassword(password) == true);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _entities.SingleOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User> CreateAsync(User entity, string password)
        {
            User user = new User()
            {
                UserUUID = Guid.NewGuid(),
                Email = entity.Email,
                UserName = entity.UserName,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Blocked = false,
                Role = Enums.ERoles.USER,
            };

            user.SetPassword(password);

            _entities.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
