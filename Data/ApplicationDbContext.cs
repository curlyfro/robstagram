using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using robstagram.Models.Entities;

namespace robstagram.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Like>()
                .HasKey(l => new {l.EntryId, l.CustomerId});

            builder.Entity<Like>()
                .HasOne(like => like.Entry)
                .WithMany(e => e.Likes)
                .HasForeignKey(like => like.EntryId);

            builder.Entity<Like>()
                .HasOne(l => l.Customer)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.CustomerId);
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Entry> Entries { get; set; }
    }
}
