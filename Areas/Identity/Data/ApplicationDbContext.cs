using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Core.Entities; // Scenario, ScenarioCategory, Classroom, StudentClassroom için
using YetenekPusulasi.Data;
using YetenekPusulasi.Models; // ApplicationUser için

namespace YetenekPusulasi.Areas.Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<StudentClassroom> StudentClassrooms { get; set; }
        public DbSet<Notification> Notifications { get; set; } // <<< YENİ EKLEME
        public DbSet<Scenario> Scenarios { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Identity tabloları için bu satır ÇOK ÖNEMLİDİR, en başta kalmalı.


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


            // Notification için ilişki (opsiyonel ama iyi pratik)
            builder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                      .WithMany() // ApplicationUser'da Notifications koleksiyonu olmayacaksa (tek yönlü)
                      .HasForeignKey(n => n.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silinirse bildirimleri de sil
            });
            

             builder.Entity<Scenario>(entity =>
            {
                entity.HasOne(s => s.Teacher)
                      .WithMany() // ApplicationUser'da Scenarios koleksiyonu olmayacaksa
                      .HasForeignKey(s => s.TeacherId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict); // Öğretmen silinirse senaryolar ne olacak? Restrict veya Cascade

                entity.HasOne(s => s.Classroom)
                      .WithMany() // Classroom'da Scenarios koleksiyonu eklenebilir: .WithMany(c => c.Scenarios)
                      .HasForeignKey(s => s.ClassroomId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade); // Sınıf silinirse senaryoları da sil
            });
        }
    }
}