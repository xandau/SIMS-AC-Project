using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI
{
    public class SIMSContext : DbContext
    {
        public DbSet<WebAPI.Models.LogEntry> logEntries { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }

        public SIMSContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(a => a.UserName).IsUnique();
            modelBuilder.Entity<User>().HasIndex(a => a.Email).IsUnique();
        }
    }
}
