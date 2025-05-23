using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPI.AuthServices;
using WebAPI.Models;

namespace WebAPI.Repository
{
    public class TicketRepository : ARepository<Ticket>, ITicketRepository
    {    
        private JwtService jwtService;

        public TicketRepository(SIMSContext context) : base(context)
        {
            _context = context;
            jwtService = new JwtService();
        }

        public override async Task<Ticket> GetAsync(long id, string access_token = "")
        {
            Ticket? entity = null;

            if (_entities.Count() == 0)
                return null;

            entity = await _entities.Include(t => t.Creator).FirstAsync(u => u.ID == id);

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

        public override async Task<Ticket> CreateAsync(Ticket entity)
        {
            Ticket ticket = new Ticket()
            {
                Title = entity.Title,
                Description = entity.Description,
                State = entity.State,
                CreationTime = DateTime.Now,
                Severity = entity.Severity,
                CVE = entity.CVE,
                CreatorID = entity.CreatorID,
                AssignedPersonID = entity.AssignedPersonID,
                ReferenceID = entity.ReferenceID
            };

            _entities.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<List<Ticket>> GetAssignedTickets(string access_token)
        {
            long id = jwtService.GetClaimsFromToken(access_token);
            if (id == 0)
                throw new Exception("No Identifier Found");
            else
                return await _entities.Where(t => t.AssignedPersonID == id).ToListAsync();
        }

        public async Task<List<Ticket>> GetCreatedTickets(string access_token)
        {
            long id = jwtService.GetClaimsFromToken(access_token);
            if (id == 0)
                throw new Exception("No Identifier Found");
            else
                return await _entities.Where(t => t.CreatorID == id).ToListAsync();
        }
    }
}
