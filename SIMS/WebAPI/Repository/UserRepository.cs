using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using System;
using System.ComponentModel.DataAnnotations;
using WebAPI.AuthServices;
using Azure.Core;

namespace WebAPI.Repository
{
    public class UserRepository : ARepository<User>
    {
        private JwtService jwtService;

        public UserRepository(SIMSContext context) : base(context)
        {
            _context = context;
            jwtService = new JwtService();
        }

        public async Task<User?> GetUserByMailAsync(string mail, string password)
        {
            User? user = await _entities.Where(u => u.Email == mail).FirstOrDefaultAsync();

            if (user != null && user.VerifyPassword(password) == true)
            {
                return user;
            }

            return null;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _entities.Where(u => u.UserName == username).FirstOrDefaultAsync();
        }

        public override async Task<User> GetAsync(long id, string access_token = "")
        {
           User? entity = await _entities.Include(u => u.CreatedTickets).FirstOrDefaultAsync(u => u.ID == id);

            if (entity is not null)
            {
                _logEntry.Add(new LogEntry()
                {
                    Level = LogLevel.Information,
                    Timestamp = DateTime.Now,
                    Message = $"Element of type {entity.GetType().Name} with ID \"{entity.ID}\" read"
                });
                await _context.SaveChangesAsync();
            }
            return entity;
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

                _logEntry.Add(new LogEntry()
                {
                    Level = LogLevel.Information,
                    Timestamp = DateTime.Now,
                    Message = $"{entity.GetType().Name} with ID \"{entity.ID}\" created"
                });
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

        // private Methods
        private void ValidateUser(User user)
        {
            var validationContext = new ValidationContext(user);
            Validator.ValidateObject(user, validationContext, validateAllProperties: true);
        }
    }
}
