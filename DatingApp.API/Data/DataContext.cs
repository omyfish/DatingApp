using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options) {}

        public DbSet<Value> Values { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Photo> Photos { get; set; }

        public DbSet<Like> Likes {get;set;}

        public DbSet<Message> Messages {get;set;}

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<Like>()
            .HasKey(e => new { e.LikerId, e.LikeeId});

            builder.Entity<Like>()
            .HasOne(e => e.Likee)
            .WithMany(e => e.Likers)
            .HasForeignKey(e => e.LikeeId)
            .OnDelete(DeleteBehavior.Restrict);

             builder.Entity<Like>()
            .HasOne(e => e.Liker)
            .WithMany(e => e.Likees)
            .HasForeignKey(e => e.LikerId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
            .HasOne(e => e.Sender)
            .WithMany(e => e.MessageSent)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
            .HasOne(e => e.Recipient)
            .WithMany(e => e.MessageReceived)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}