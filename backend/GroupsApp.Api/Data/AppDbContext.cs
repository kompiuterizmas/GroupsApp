using System;
using System.Collections.Generic;
using System.Text.Json;
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

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<GroupMember> GroupMembers { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite key for join table
            modelBuilder.Entity<GroupMember>()
                .HasKey(gm => new { gm.GroupId, gm.UserId });

            // Auto-generate IDs
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Group>()
                .Property(g => g.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd();

            // Persist SplitDetails as JSON so it can be read back for balance calculation
            modelBuilder.Entity<Transaction>()
                .Property(t => t.SplitDetails)
                .HasConversion(
                    dict => JsonSerializer.Serialize(dict, (JsonSerializerOptions?)null),
                    json => string.IsNullOrEmpty(json)
                              ? new Dictionary<int, decimal>()
                              : JsonSerializer.Deserialize<Dictionary<int, decimal>>(json, (JsonSerializerOptions?)null)
                );

            // Relationships
            modelBuilder.Entity<Group>()
                .HasMany(g => g.GroupMembers)
                .WithOne(gm => gm.Group)
                .HasForeignKey(gm => gm.GroupId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.PayerTransactions)
                .WithOne(t => t.Payer)
                .HasForeignKey(t => t.PayerUserId);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Transactions)
                .WithOne(t => t.Group)
                .HasForeignKey(t => t.GroupId);
        }
    }
}
