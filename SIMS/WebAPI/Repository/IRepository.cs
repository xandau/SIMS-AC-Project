namespace WebAPI.Repository
{
    public interface IRepository<Entity> where Entity : class
    {
        Task<Entity> CreateAsync(Entity entity);
        Task UpdateAsync(Entity entity);
        Task DeleteAsync(Entity entity);
        Task<Entity> GetAsync(long id, string access_token = "");
        Task<List<Entity>> GetAllAsync();
    }
}
