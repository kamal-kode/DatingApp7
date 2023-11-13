using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AppUser> Users { get; set; }
    public DbSet<UserLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserLike>()
          .HasKey(k => new { k.SourceUserId, k.TargetUserId });

        modelBuilder.Entity<UserLike>()
         .HasOne(s => s.SourceUser)
         .WithMany(l => l.LikedUsers)
         .HasForeignKey(s => s.SourceUserId)
         .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserLike>()
         .HasOne(s => s.TargetUser)
         .WithMany(l => l.LikedByUsers)
         .HasForeignKey(s => s.TargetUserId)
         .OnDelete(DeleteBehavior.Cascade); // MS sql will not work with two cascade need to add Noaction

        modelBuilder.Entity<Message>()
        .HasOne(x => x.Recipient)
        .WithMany(m => m.MessagesReceived)
        .OnDelete(DeleteBehavior.Restrict); // sender should not be able to delete the sent message in case profile deleted
    
        modelBuilder.Entity<Message>()
        .HasOne(x => x.Sender)
        .WithMany(m => m.MessagesSent)
        .OnDelete(DeleteBehavior.Restrict); // receiver should not be able to delete the sent message in case profile deleted
    }
}
