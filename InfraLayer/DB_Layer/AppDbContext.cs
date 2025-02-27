using Microsoft.EntityFrameworkCore;
using DomainLayer.Entities;

namespace InfraLayer.DB_Layer
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserDetails> Users { get; set; }
        public DbSet<TaskDetails> Tasks { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TaskDetails>().HasKey(p => p.TaskId);
            
        }
    }
}
