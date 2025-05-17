// backend/GroupsApp.Api/Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using GroupsApp.Api.Models;

namespace GroupsApp.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<GroupMember> GroupMembers { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite primary key for join entity
            modelBuilder.Entity<GroupMember>()
                .HasKey(gm => new { gm.GroupId, gm.UserId });

            // One Group → many GroupMembers
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.GroupMembers)
                .HasForeignKey(gm => gm.GroupId);

            // One User → many GroupMembers
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMembers)
                .HasForeignKey(gm => gm.UserId);

            // Configure Transaction ↔ Group relationship
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Group)
                .WithMany(g => g.Transactions)
                .HasForeignKey(t => t.GroupId);

            // Configure Transaction ↔ Payer (User) relationship
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Payer)
                .WithMany(u => u.PayerTransactions)
                .HasForeignKey(t => t.PayerUserId);
        }
    }
}
