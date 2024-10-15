
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Repository
{
    public abstract class ARepository<Entity> : IRepository<Entity> where Entity : class
    {
        protected SIMSContext _context;
        protected DbSet<Entity> _entities;

        protected ARepository(SIMSContext context)
        {
            _context = context;
            _entities = context.Set<Entity>();
        }

        public virtual async Task<Entity> CreateAsync(Entity entity)
        {
            _entities.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(Entity entity)
        {
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Entity>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }

        public async Task<Entity> GetAsync(long id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task UpdateAsync(Entity entity)
        {
            _context.ChangeTracker.Clear();
            _entities.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
