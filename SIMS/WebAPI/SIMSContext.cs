using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI
{
    public class SIMSContext : DbContext
    {
        public DbSet<LogEntry> logEntries { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }

        public SIMSContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(a => a.UserName).IsUnique();
            modelBuilder.Entity<User>().HasIndex(a => a.Email).IsUnique();

            modelBuilder.Entity<User>().HasKey(a => a.ID);
            modelBuilder.Entity<Ticket>().HasKey(a => a.ID);
            modelBuilder.Entity<LogEntry>().HasKey(a => a.ID);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Creator)
                .WithMany(u => u.CreatedTickets)
                .HasForeignKey(t => t.CreatorID);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedPerson)
                .WithMany(u => u.AssignedTickets)
                .HasForeignKey(t => t.AssignedPersonID);
        }
    }
}
