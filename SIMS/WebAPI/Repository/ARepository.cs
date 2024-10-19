
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Repository
{
    public abstract class ARepository<Entity> : IRepository<Entity> where Entity : AItem
    {
        protected SIMSContext _context;
        protected DbSet<Entity> _entities;
        protected DbSet<LogEntry> _logEntry;

        protected ARepository(SIMSContext context)
        {
            _context = context;
            _entities = context.Set<Entity>();
            _logEntry = context.Set<LogEntry>();
        }

        public virtual async Task<Entity> CreateAsync(Entity entity)
        {
            _entities.Add(entity);
            _logEntry.Add(new LogEntry() { Level = LogLevel.Information, Timestamp = DateTime.Now, Message = $"{entity.GetType().Name} created"});
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(Entity entity)
        {
            _entities.Remove(entity);
            _logEntry.Add(new LogEntry() { Level = LogLevel.Information, Timestamp = DateTime.Now, Message = $"{entity.GetType().Name} with ID: {entity.ID} deleted"});
            await _context.SaveChangesAsync();
        }

        public async Task<List<Entity>> GetAllAsync()
        {
            if (_entities.Count() == 0)
                return null;

            List<Entity> entities = await _entities.ToListAsync();

            _logEntry.Add(new LogEntry()
            {
                Level = LogLevel.Information,
                Timestamp = DateTime.Now,
                Message = $"{entities.Count} elements of type {entities.First().GetType().Name} read"
            });

            await _context.SaveChangesAsync();

            return entities; 
        }

        public virtual async Task<Entity> GetAsync(long id)
        {
            Entity? entity = await _entities.FindAsync(id);

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

        public async Task UpdateAsync(Entity entity)
        {
            _context.ChangeTracker.Clear();
            _entities.Update(entity);
            _logEntry.Add(new LogEntry() 
            { 
                Level = LogLevel.Information, 
                Timestamp = DateTime.Now, 
                Message = $"{entity.GetType().Name} with ID: {entity.ID} updated" 
            });
            await _context.SaveChangesAsync();
        }
    }
}
