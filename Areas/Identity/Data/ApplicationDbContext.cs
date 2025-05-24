// Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Core.Entities; // Ekledik

namespace YetenekPusulasi.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser> // IdentityUser, IdentityRole vb. buradan gelir
    {
        public DbSet<Scenario> Scenarios { get; set; } // Ekledik
        public DbSet<ScenarioCategory> ScenarioCategories { get; set; } // Ekledik

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Identity tabloları için bu önemli

            // Senaryo ve Kategori ilişkisi
            builder.Entity<Scenario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Text).IsRequired();
                entity.Property(e => e.TargetSkill).HasMaxLength(100);

                entity.HasOne(d => d.ScenarioCategory)
                    .WithMany(p => p.Scenarios)
                    .HasForeignKey(d => d.ScenarioCategoryId)
                    .OnDelete(DeleteBehavior.Restrict); // Kategori silinirse senaryolar ne olacak?
            });

            builder.Entity<ScenarioCategory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                // Örnek veri (Seed data)
                entity.HasData(
                    new ScenarioCategory { Id = 1, Name = "Problem Çözme", Description = "Problem çözme yetenekleri." },
                    new ScenarioCategory { Id = 2, Name = "Empati", Description = "Empati kurma becerileri." },
                    new ScenarioCategory { Id = 3, Name = "Analitik Düşünme", Description = "Analitik düşünme senaryoları." }
                );
            });
        }
    }
}