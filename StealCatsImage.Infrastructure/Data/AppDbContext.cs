using Microsoft.EntityFrameworkCore;
using StealCatsImage.Domain.Entities;

namespace StealCatsImage.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<CatEntity> Cats => Set<CatEntity>();
        public DbSet<TagEntity> Tags => Set<TagEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CatEntity>(e =>
            {
                e.ToTable("Cats");
                e.HasKey(x => x.Id);

                e.Property(x => x.CatId)
                    .IsRequired()
                    .HasMaxLength(64);

                e.HasIndex(x => x.CatId).IsUnique(); //no duplicates

                e.Property(x => x.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(2048);

                e.Property(x => x.Created)
                    .IsRequired();

                // many-to-many
                e.HasMany(x => x.Tags)
                 .WithMany(x => x.Cats)
                 .UsingEntity(j => j.ToTable("CatTags"));
            });

            modelBuilder.Entity<TagEntity>(e =>
            {
                e.ToTable("Tags");

                e.HasKey(x => x.Id);

                e.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                // no duplicate personality tag
                e.HasIndex(x => x.Name).IsUnique();

                e.Property(x => x.Created)
                    .IsRequired();
            });
        }
    }
}
