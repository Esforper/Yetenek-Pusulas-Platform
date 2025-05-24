// Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Models; // ApplicationUser için

namespace YetenekPusulasi.Data
{
    // IdentityDbContext<ApplicationUser> olarak değiştirin
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Scenario> Scenarios { get; set; }
        public DbSet<ScenarioCategory> ScenarioCategories { get; set; }
        public DbSet<Classroom> Classrooms { get; set; } // YENİ
        public DbSet<StudentClassroom> StudentClassrooms { get; set; } // YENİ

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Senaryo ve Kategori ilişkisi (önceki gibi)
            builder.Entity<Scenario>(entity =>
            {
                // ... (önceki konfigürasyonlar)
            });
            builder.Entity<ScenarioCategory>(entity =>
            {
                // ... (önceki konfigürasyonlar)
            });

            // Classroom Entity Konfigürasyonu (YENİ)
            builder.Entity<Classroom>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.ParticipationCode).IsRequired().HasMaxLength(10); // Kod uzunluğu
                entity.HasIndex(c => c.ParticipationCode).IsUnique(); // Katılım kodu benzersiz olmalı

                entity.HasOne(c => c.Teacher)
                      .WithMany() // Bir öğretmenin birden fazla sınıfı olabilir
                      .HasForeignKey(c => c.TeacherId)
                      .IsRequired();
            });

            // StudentClassroom (Çoğa Çok İlişki) Konfigürasyonu (YENİ)
            builder.Entity<StudentClassroom>(entity =>
            {
                entity.HasKey(sc => new { sc.StudentId, sc.ClassroomId }); // Bileşik anahtar

                entity.HasOne(sc => sc.Student)
                      .WithMany() // Bir öğrencinin birden fazla StudentClassroom kaydı olabilir
                      .HasForeignKey(sc => sc.StudentId)
                      .OnDelete(DeleteBehavior.Restrict); // Öğrenci silinirse ne olacak?

                entity.HasOne(sc => sc.Classroom)
                      .WithMany(c => c.StudentClassrooms) // Bir sınıfın birden fazla StudentClassroom kaydı
                      .HasForeignKey(sc => sc.ClassroomId)
                      .OnDelete(DeleteBehavior.Cascade); // Sınıf silinirse ilişkili kayıtlar da silinsin
            });

            // Roller için Seed Data (Program.cs'te de yapılabilir)
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "Teacher", NormalizedName = "TEACHER" },
                new IdentityRole { Id = "3", Name = "Student", NormalizedName = "STUDENT" }
            );
        }
    }
}