using IdentityCredentialService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityCredentialService.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(q => q.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}
