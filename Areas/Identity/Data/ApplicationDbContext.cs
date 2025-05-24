using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Core.Entities; // Scenario, ScenarioCategory, Classroom, StudentClassroom için
using YetenekPusulasi.Data;
using YetenekPusulasi.Models; // ApplicationUser için

namespace YetenekPusulasi.Areas.Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Scenario> Scenarios { get; set; }
        public DbSet<ScenarioCategory> ScenarioCategories { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<StudentClassroom> StudentClassrooms { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Identity tabloları için bu satır ÇOK ÖNEMLİDİR, en başta kalmalı.

            // --- Senaryo ve Kategori İlişkileri ---
            builder.Entity<Scenario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Text).IsRequired();
                entity.Property(e => e.TargetSkill).HasMaxLength(100);

                entity.HasOne(d => d.ScenarioCategory)
                    .WithMany(p => p.Scenarios)
                    .HasForeignKey(d => d.ScenarioCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ScenarioCategory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                // Örnek KATEGORİ verisi (Seed data) - ID'ler sabit olmalı
                // Eğer kategori ID'leri de dinamikse, bunları da Program.cs'te oluşturmayı düşünebilirsiniz.
                // Ancak genellikle temel kategoriler sabit ID'lerle seed edilebilir.
                entity.HasData(
                    new ScenarioCategory { Id = 1, Name = "Problem Çözme", Description = "Problem çözme yetenekleri." },
                    new ScenarioCategory { Id = 2, Name = "Empati", Description = "Empati kurma becerileri." },
                    new ScenarioCategory { Id = 3, Name = "Analitik Düşünme", Description = "Analitik düşünme senaryoları." }
                );
            });

            // --- Classroom Entity Konfigürasyonu ---
            builder.Entity<Classroom>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Description).HasMaxLength(500);
                entity.Property(c => c.ParticipationCode).IsRequired().HasMaxLength(10);
                entity.HasIndex(c => c.ParticipationCode).IsUnique();

                entity.HasOne(c => c.Teacher)
                      .WithMany()
                      .HasForeignKey(c => c.TeacherId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- StudentClassroom (Çoğa Çok İlişki) Konfigürasyonu ---
            builder.Entity<StudentClassroom>(entity =>
            {
                entity.HasKey(sc => new { sc.StudentId, sc.ClassroomId });

                entity.HasOne(sc => sc.Student)
                      .WithMany()
                      .HasForeignKey(sc => sc.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sc => sc.Classroom)
                      .WithMany(c => c.StudentClassrooms)
                      .HasForeignKey(sc => sc.ClassroomId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ROLLER İÇİN HasData KISMI TAMAMEN KALDIRILDI.
            // Roller Program.cs veya bir DataSeeder servisi aracılığıyla oluşturulacak.
        }
    }
}