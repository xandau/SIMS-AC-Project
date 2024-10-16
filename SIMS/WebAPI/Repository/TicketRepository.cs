using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Repository
{
    public class TicketRepository : ARepository<Ticket>
    {
        public TicketRepository(SIMSContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<Ticket> GetAsync(long id)
        {
            Ticket? entity = await _entities.Include(t => t.Creator).FirstAsync(u => u.ID == id);

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
                AssignedPersonID = entity.AssignedPersonID
            };

            _entities.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }
    }
}
