// Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Entities.Scenarios; // Somut senaryo sınıfları için
using YetenekPusulasi.Data; // ApplicationUser için (Eğer bu namespace'deyse)
// YetenekPusulasi.Models namespace'i ApplicationUser için kullanılıyorsa o da kalmalı.
// Projenizin ApplicationUser'ının bulunduğu doğru namespace'i kontrol edin.

namespace YetenekPusulasi.Areas.Identity.Data // VEYA DbContext'inizin olduğu doğru namespace
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<StudentClassroom> StudentClassrooms { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Scenario> Scenarios { get; set; }
        public DbSet<AnalysisResult> AnalysisResults { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Bu her zaman en başta kalmalı.

            // --- Classroom Entity Konfigürasyonu ---
            builder.Entity<Classroom>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Description).HasMaxLength(500); // Nullable olmalı (string?)
                entity.Property(c => c.ParticipationCode).IsRequired().HasMaxLength(10); // Uzunluğu gözden geçirin
                entity.HasIndex(c => c.ParticipationCode).IsUnique();

                entity.HasOne(c => c.Teacher)
                      .WithMany() // ApplicationUser'da Classrooms koleksiyonu yoksa
                      .HasForeignKey(c => c.TeacherId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                // Classroom'dan Scenarios'a ilişki (Eğer Classroom entity'sinde ICollection<Scenario> Scenarios varsa)
                entity.HasMany(c => c.Scenarios)
                      .WithOne(s => s.Classroom)
                      .HasForeignKey(s => s.ClassroomId)
                      .OnDelete(DeleteBehavior.Cascade); // Sınıf silinirse ilişkili senaryolar da silinsin
            });

            // --- StudentClassroom (Çoğa Çok İlişki) Konfigürasyonu ---
            builder.Entity<StudentClassroom>(entity =>
            {
                entity.HasKey(sc => new { sc.StudentId, sc.ClassroomId });

                entity.HasOne(sc => sc.Student) // StudentClassroom -> Student ilişkisi
                    .WithMany()               // <<< DEĞİŞİKLİK BURADA: ApplicationUser tarafında koleksiyon yok
                    .HasForeignKey(sc => sc.StudentId)
                    .IsRequired() // Genellikle bir StudentClassroom kaydının bir Student'ı olmalı
                    .OnDelete(DeleteBehavior.Cascade); // Öğrenci silinirse, sınıfla olan bağlantısı da silinsin

                entity.HasOne(sc => sc.Classroom) // StudentClassroom -> Classroom ilişkisi
                    .WithMany(c => c.StudentClassrooms) // Classroom'da StudentClassrooms koleksiyonu var
                    .HasForeignKey(sc => sc.ClassroomId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade); // Sınıf silinirse, öğrenci bağlantıları da silinsin
            });

            // --- Notification Entity Konfigürasyonu ---
            builder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // --- Scenario Entity Konfigürasyonu (TEK BLOK İÇİNDE) ---
            builder.Entity<Scenario>(entity =>
            {
                // İlişkiler
                entity.HasOne(s => s.Teacher)
                    .WithMany() // ApplicationUser'da Scenarios koleksiyonu yoksa
                    .HasForeignKey(s => s.TeacherId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                // Classroom ile ilişki zaten Classroom entity'sinde HasMany ile kuruldu,
                // burada tekrar HasOne/WithMany belirtmeye gerek yok eğer navigasyon property'leri doğruysa.
                // EF Core bunu genellikle otomatik olarak anlar.
                // Ama eğer açıkça belirtmek isterseniz:
                // entity.HasOne(s => s.Classroom)
                //    .WithMany(c => c.Scenarios) // Classroom'da Scenarios koleksiyonu varsa
                //    .HasForeignKey(s => s.ClassroomId)
                //    .IsRequired() // Bu zaten Classroom tarafında HasForeignKey ile kuruldu.
                //    .OnDelete(DeleteBehavior.Cascade);

                // TPH (Table-Per-Hierarchy) için Discriminator yapılandırması
                // Bu, Scenario'nun kendisi için değil, genel miras hiyerarşisi için bir kez tanımlanır.
                // Bu yüzden bunu Entity<Scenario>() bloğunun dışına almak daha doğru olabilir,
                // ya da burada kalabilir ama ikincil Entity<Scenario> bloğu olmamalı.
                // Şimdilik burada bırakalım ama diğer bloğu sileceğiz.
            });

            // TPH (Table-Per-Hierarchy) için Discriminator yapılandırması
            // Bu, yukarıdaki Entity<Scenario>() bloğundan SONRA ve ayrı olarak yapılmalı.
            builder.Entity<Scenario>() // Bu satırın kendisi zaten bir konfigürasyon başlatır.
                .HasDiscriminator<string>("ScenarioDiscriminator")
                .HasValue<GenericScenario>(nameof(GenericScenario))
                .HasValue<ProblemSolvingScenario>(nameof(ProblemSolvingScenario))
                .HasValue<DecisionMakingScenario>(nameof(DecisionMakingScenario))
                .HasValue<AnalyticalThinkingScenario>(nameof(AnalyticalThinkingScenario))
                .HasValue<EmpathyDevelopmentScenario>(nameof(EmpathyDevelopmentScenario))
                .HasValue<CreativeThinkingScenario>(nameof(CreativeThinkingScenario));


            // --- AnalysisResult Entity Konfigürasyonu ---
             // StudentAnswer için ilişkiler
            builder.Entity<StudentAnswer>(entity =>
            {
                entity.HasOne(sa => sa.Student)
                    .WithMany() // Bu StudentClassrooms olmamalı, ya boş ya da Student'ta Answers koleksiyonu
                    .HasForeignKey(sa => sa.StudentId)
                    .OnDelete(DeleteBehavior.Restrict); // Öğrenci silinirse cevapları ne olacak?

                entity.HasOne(sa => sa.Scenario) // IScenario tipinde olduğu için dikkat
                    .WithMany() // Scenario'da StudentAnswers koleksiyonu yoksa
                    .HasForeignKey(sa => sa.ScenarioId)
                    .OnDelete(DeleteBehavior.Cascade); // Senaryo silinirse cevapları da sil

                // StudentAnswer ve AnalysisResult arasında bire bir ilişki
                entity.HasOne(sa => sa.AnalysisResult)
                    .WithOne(ar => ar.StudentAnswer) // AnalysisResult'ta StudentAnswer navigasyonu olmalı
                    .HasForeignKey<AnalysisResult>(ar => ar.StudentAnswerId); // AnalysisResult'taki FK
            });

            builder.Entity<AnalysisResult>(entity =>
            {
                entity.HasKey(ar => ar.Id); // Primary Key

                // AnalysisResult'ın StudentAnswer ile birebir ilişkisi
                // StudentAnswerId, AnalysisResult'ta Foreign Key olacak
                // ve StudentAnswer'ın Id'sine işaret edecek.
                // StudentAnswer tarafında AnalysisResult nullable (bir cevap henüz analiz edilmemiş olabilir)
                // AnalysisResult tarafında StudentAnswer zorunlu (bir analiz mutlaka bir cevaba ait olmalı)
                entity.HasOne(ar => ar.StudentAnswer) // AnalysisResult -> StudentAnswer navigasyonu
                      .WithOne(sa => sa.AnalysisResult) // StudentAnswer -> AnalysisResult navigasyonu
                      .HasForeignKey<AnalysisResult>(ar => ar.StudentAnswerId) // FK AnalysisResult'ta
                      .IsRequired() // Bir analiz sonucu mutlaka bir öğrenci cevabına bağlı olmalı
                      .OnDelete(DeleteBehavior.Cascade); // Cevap silinirse analiz sonucu da silinsin
            });


        }
    }
}