// Infrastructure/Data/ApplicationDbContext.cs (WebApp/Areas/Identity/Data altında olabilir)
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Core.Entities;
namespace YetenekPusulasi.Infrastructure.Data // Veya WebApp.Areas.Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Scenario> Scenarios { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<StudentClassroom> StudentClassrooms { get; set; }
        // Diğer DbSet'ler

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Entity konfigürasyonları (Fluent API)
            builder.Entity<StudentClassroom>().HasKey(sc => new { sc.StudentId, sc.ClassroomId });
            builder.Entity<Classroom>().HasIndex(c => c.ParticipationCode).IsUnique();
            // ... diğer ilişkiler ve konfigürasyonlar
        }
    }
}



/*  --- Classroom Entity Konfigürasyonu ---
entity.HasOne(c => c.Teacher)
      .WithMany(u => u.TaughtClasses) // ApplicationUser.TaughtClasses ile ilişkilendir
      .HasForeignKey(c => c.TeacherId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Restrict);

--- StudentClassroom (Çoğa Çok İlişki) Konfigürasyonu ---
entity.HasOne(sc => sc.Student)
      .WithMany(u => u.EnrolledStudentClassrooms) // ApplicationUser.EnrolledStudentClassrooms ile ilişkilendir
      .HasForeignKey(sc => sc.StudentId)
      .OnDelete(DeleteBehavior.Cascade);
*/