using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using System;
using System.ComponentModel.DataAnnotations;

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
            User user = await _entities.Where(u => u.Email == mail).FirstAsync();

            if (user != null && user.VerifyPassword(password) == true)
            {
                return user;
            }

            return null;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _entities.Where(u => u.UserName == username).FirstAsync();
        }

        public override async Task<User> CreateAsync(User entity)
        {
            // Validate the user entity
            ValidateUser(entity);

            try
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

                user.SetPassword(entity.Password);

                _entities.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                if (sqlEx.Number == 2601 && sqlEx.Message.Contains("IX_USERS_EMAIL"))
                {
                    throw new InvalidOperationException("Email/User already exists.");
                }
                else if (sqlEx.Number == 2601 && sqlEx.Message.Contains("IX_USERS_USERNAME"))
                {
                    throw new InvalidOperationException("Email/User already exists.");
                }
                else
                {
                    throw new InvalidOperationException("Error, womp womp.");
                }
            }
        }

        private void ValidateUser(User user)
        {
            var validationContext = new ValidationContext(user);
            Validator.ValidateObject(user, validationContext, validateAllProperties: true);
        }
    }
}
