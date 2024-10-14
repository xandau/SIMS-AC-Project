using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI
{
    public class SIMSContext : DbContext
    {
        public DbSet<WebAPI.Models.LogEntry> logEntries { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }

        public SIMSContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            IConfigurationProvider secretProvider = config.Providers.First();
            secretProvider.TryGet("ConnectionStrings:SQL", out var secretData);

            

            optionsBuilder.UseSqlServer(secretData);
        }
    }
}
