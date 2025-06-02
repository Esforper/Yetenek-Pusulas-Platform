using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Core.Entities;

namespace YetenekPusulasi.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<StudentClassroom> StudentClassrooms { get; set; }
        public DbSet<Scenario> Scenarios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<StudentClassroom>()
                .HasKey(sc => new { sc.StudentId, sc.ClassroomId });

            builder.Entity<Classroom>()
                .HasIndex(c => c.ParticipationCode)
                .IsUnique();
        }
    }
} 