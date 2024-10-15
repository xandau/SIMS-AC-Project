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
    }
}
