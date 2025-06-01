// WebApp/Program.cs

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Core.Entities; // ApplicationUser, Scenario, Classroom vb.
using YetenekPusulasi.Core.Factories; // SimpleScenarioFactory
using YetenekPusulasi.Core.Interfaces.Factories; // IScenarioFactory
using YetenekPusulasi.Core.Interfaces.Repositories; // IRepository, IScenarioRepository, IClassroomRepository, IUserRepository
using YetenekPusulasi.Core.Interfaces.Services; // IScenarioService (eğer tanımladıysanız), IClassroomService
using YetenekPusulasi.Core.Interfaces.Strategies; // IScenarioGenerationStrategy
using YetenekPusulasi.Core.Interfaces.Observers; // IClassroomObserver
using YetenekPusulasi.Core.Services; // ScenarioOrchestrationService, ClassroomManagementService
using YetenekPusulasi.Core.Strategies; // RuleBasedScenarioStrategy
using YetenekPusulasi.Core.Adapters; // ExternalAIServiceAdapter ve varsayımsal ThirdPartyAI
using YetenekPusulasi.Core.Facades; // StudentEnrollmentFacade
using YetenekPusulasi.Core.Observers; // AuditLogClassroomObserver
using YetenekPusulasi.Core.Singletons; // AppConfiguration (DI için opsiyonel)
using YetenekPusulasi.Infrastructure.Data; // ApplicationDbContext (WebApp/Areas/Identity/Data altında olduğunu varsayıyoruz)
using YetenekPusulasi.Infrastructure.Data.Repositories; // EfRepository, ScenarioRepository, ClassroomRepository, UserRepository

// --- Namespace Düzeltmesi ---
// ApplicationUser'ın bulunduğu namespace'e göre bu using'i ayarlayın.
// Eğer ApplicationUser.cs dosyanız WebApp projesinin kök Data klasöründeyse:
// using YetenekPusulasi.Data;
// Eğer Core projesinin Entities klasöründeyse (yukarıdaki gibi):
// using YetenekPusulasi.Core.Entities;
// Bu örnekte Core.Entities altında olduğunu varsayıyorum.

var builder = WebApplication.CreateBuilder(args);

// --- Veritabanı Bağlantısı ve DbContext ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure( // Geçici hatalara karşı dayanıklılık
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        );
    }));

// --- ASP.NET Core Identity ---
// ApplicationUser tipi Core.Entities altında tanımlandıysa, using doğru.
builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
        options.SignIn.RequireConfirmedAccount = false; // E-posta onayı gereksinimini kapattık
        // Diğer Identity ayarları (şifre politikası vb.) buraya eklenebilir
        // options.Password.RequireDigit = true;
        // options.Password.RequiredLength = 8;
    })
    .AddRoles<IdentityRole>() // Rol yönetimini etkinleştir
    .AddEntityFrameworkStores<ApplicationDbContext>(); // Identity'nin hangi DbContext'i kullanacağını belirt

// --- MVC ve Razor Pages ---
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Identity UI Razor Pages kullandığı için bu genellikle gereklidir

// --- Depo (Repository) Kayıtları ---
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>)); // Generic repository
builder.Services.AddScoped<IScenarioRepository, ScenarioRepository>();
builder.Services.AddScoped<IClassroomRepository, ClassroomRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>(); // Eğer IUserRepository ve UserRepository tanımladıysanız

// --- Fabrika (Factory) Kayıtları ---
builder.Services.AddScoped<IScenarioFactory, SimpleScenarioFactory>();

// --- Strateji (Strategy) Kayıtları ---
builder.Services.AddScoped<RuleBasedScenarioStrategy>();
builder.Services.AddScoped<ThirdPartyAI.AdvancedAISdk>(); // Adapter için varsayımsal dış servis
builder.Services.AddScoped<ExternalAIServiceAdapter>();

// IScenarioGenerationStrategy arayüzü için birden fazla implementasyon kaydediyoruz.
// Bunlar ScenarioOrchestrationService'e IEnumerable<IScenarioGenerationStrategy> olarak enjekte edilebilir
// veya ScenarioService içinde belirli bir tip (örn: RuleBasedScenarioStrategy) doğrudan enjekte alınabilir.
builder.Services.AddScoped<IScenarioGenerationStrategy, RuleBasedScenarioStrategy>(sp => sp.GetRequiredService<RuleBasedScenarioStrategy>());
builder.Services.AddScoped<IScenarioGenerationStrategy, ExternalAIServiceAdapter>(sp => sp.GetRequiredService<ExternalAIServiceAdapter>());

// --- Servis Katmanı Kayıtları ---
builder.Services.AddScoped<ScenarioOrchestrationService>(sp =>
    new ScenarioOrchestrationService(
        sp.GetRequiredService<IScenarioRepository>(),
        // Varsayılan stratejiyi burada belirleyebilirsiniz veya controller'dan seçtirebilirsiniz.
        // Örneğin, RuleBased varsayılan olsun:
        sp.GetRequiredService<RuleBasedScenarioStrategy>()
    ));

// ClassroomManagementService için IClassroomObserver'ları da enjekte ediyoruz.
builder.Services.AddScoped<ClassroomManagementService>(); // IClassroomObserver'ları constructor'dan alacak

// IScenarioService ve IClassroomService gibi arayüzleriniz varsa:
// builder.Services.AddScoped<IScenarioService, ScenarioOrchestrationService>();
// builder.Services.AddScoped<IClassroomService, ClassroomManagementService>();


// --- Gözlemci (Observer) Kayıtları ---
// ClassroomManagementService'e IEnumerable<IClassroomObserver> olarak enjekte edilecekler.
builder.Services.AddScoped<IClassroomObserver, AuditLogClassroomObserver>();
// builder.Services.AddScoped<IClassroomObserver, AdminNotifierObserver>(); // Eğer varsa

// --- Facade Kayıtları ---
builder.Services.AddScoped<StudentEnrollmentFacade>();

// --- Singleton Kayıtları (Opsiyonel) ---
// AppConfiguration zaten static Instance ile erişildiği için DI'a kaydetmek şart değil,
// ama test edilebilirlik veya farklı konfigürasyonlar için istenirse eklenebilir.
// builder.Services.AddSingleton<AppConfiguration>(AppConfiguration.Instance);
// Veya doğrudan:
// builder.Services.AddSingleton<AppConfiguration>(); // Eğer constructor'ı public ve parametresizse


// Geliştirme ortamı için özel servisler
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter(); // EF Core migration hataları için daha iyi sayfa
}


var app = builder.Build();

// --- Veritabanı Seed İşlemleri (Roller vb.) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        // Bu metodu Program.cs'in altında veya ayrı bir static class'ta tanımlayın
        await SeedDefaultRolesAsync(roleManager);
        // await SeedDefaultAdminUserAsync(userManager, roleManager); // İsteğe bağlı varsayılan admin
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
        // Geliştirme aşamasında hatayı görmek için fırlatabilirsiniz:
        // throw;
    }
}


// --- HTTP Request Pipeline Konfigürasyonu ---
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // app.Services.AddDatabaseDeveloperPageExceptionFilter() ile birlikte kullanılır
    app.UseDeveloperExceptionPage(); // Daha detaylı hata sayfası
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Production için genel hata sayfası
    app.UseHsts(); // HTTPS yönlendirmesini zorunlu kıl
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // wwwroot klasöründeki statik dosyalar için (CSS, JS, resimler)

app.UseRouting(); // Routing middleware'i

app.UseAuthentication(); // Kimlik doğrulama middleware'i (UseAuthorization'dan ÖNCE olmalı)
app.UseAuthorization();  // Yetkilendirme middleware'i

// Endpoint'leri map et
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // Identity UI Razor Pages için bu route EKLENMELİ

app.Run();


// --- Yardımcı Seed Metotları (Program.cs sonuna veya ayrı bir static sınıfa) ---
async Task SeedDefaultRolesAsync(RoleManager<IdentityRole> roleManager)
{
    string[] roleNames = { "Admin", "Teacher", "Student" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

// İsteğe Bağlı: Varsayılan bir Admin kullanıcısı oluşturmak için
/*
async Task SeedDefaultAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
{
    var adminEmail = "admin@example.com";
    var adminPassword = "AdminPassword123!"; // Güçlü bir şifre seçin ve güvenli bir yerde saklayın (örn: User Secrets)

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true // Admin kullanıcısının e-postası onaylı olsun
        };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            if (await roleManager.RoleExistsAsync("Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
*/