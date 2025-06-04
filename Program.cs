
using YetenekPusulasi.Core.Interfaces.Services;
using YetenekPusulasi.Core.Services;
using YetenekPusulasi.Data; // ApplicationDbContext için
using Microsoft.EntityFrameworkCore; // EF Core için
using Microsoft.AspNetCore.Identity.Data;
using YetenekPusulasi.Models;
using Microsoft.AspNetCore.Identity; // Identity için
using YetenekPusulasi.Core.Factories;
// using Microsoft.AspNetCore.Authentication.JwtBearer; // API için JWT kullanacaksanız
// using Microsoft.IdentityModel.Tokens; // JWT için
// using System.Text; // JWT için
using YetenekPusulasi.Core.Services;
using YetenekPusulasi.Core.Events;
using YetenekPusulasi.Core.Observers;
using YetenekPusulasi.Core.Interfaces.AI;
using YetenekPusulasi.Infrastructure.AIAdapters;
using YetenekPusulasi.Core.AIStrategies;
using YetenekPusulasi.Core.Interfaces.Strategies;
using YetenekPusulasi.Core.Strategies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<YetenekPusulasi.Areas.Identity.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
// builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<IClassroomService, ClassroomService>();
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false) // IdentityUser'ı kendi ApplicationUser'ınızla değiştirebilirsiniz
    .AddRoles<IdentityRole>() // Rolleri ekledik
    .AddEntityFrameworkStores<YetenekPusulasi.Areas.Identity.Data.ApplicationDbContext>();

builder.Services.AddControllersWithViews(); // Eğer MVC View'ları da kullanacaksanız
builder.Services.AddScoped<INotificationService, NotificationService>();



/// BU KISIM OBSERVER DESENİ İÇİN EKLENDİ
/// ÖĞRENCİ GİRİŞ YAPTIKTAN SONRA ÖĞRETMENE BİLDİRİM GÖNDERMEK İÇİN
/// 

// ClassroomService kaydınız zaten olmalı:
// builder.Services.AddScoped<IClassroomService, ClassroomService>();

// Notification Servisi
builder.Services.AddScoped<INotificationService, NotificationService>();

// Observer ve Notifier
builder.Services.AddScoped<StudentJoinedClassroomNotifier>(); // Veya AddSingleton

// Observer implementasyonlarını kaydet
builder.Services.AddScoped<IStudentJoinedObserver, LogStudentJoinObserver>();
builder.Services.AddScoped<IStudentJoinedObserver, TeacherNotificationObserver>();




/// ScenarioService ve Factory için
/// Factory Deseni Kullanıldı.
/// SenerioService, ScenarioFactory ve Observer'ı ekliyoruz
builder.Services.AddScoped<IScenarioService, ScenarioService>();
builder.Services.AddScoped<ScenarioFactory>(); // Factory'yi Scoped olarak kaydediyoruz.





// ...
// AI Adapter'ları
builder.Services.AddScoped<IAIModelAdapter, OpenAIAdapter>();
builder.Services.AddScoped<IAIModelAdapter, GoogleGeminiAdapter>();
// Eklediğiniz her yeni AI adapter için bir satır

// AI Analysis Stratejileri
builder.Services.AddScoped<IAnalysisStrategy, OpenAIAnalysisStrategy>();
builder.Services.AddScoped<IAnalysisStrategy, GeminiAnalysisStrategy>();
// Eklediğiniz her yeni strateji için bir satır

// Ana Analiz Servisi
builder.Services.AddScoped<IAnalysisService, AnalysisService>();

// IHttpClientFactory (Gerçek API çağrıları için OpenAIAdapter gibi yerlerde gerekecek)
builder.Services.AddHttpClient(); // Temel AddHttpClient







// Initial Prompt Generation Strategies
builder.Services.AddScoped<ManualInitialPromptStrategy>();
// AIInitialPromptGenerationStrategy, IAIModelAdapter'a bağımlı.
// Hangi adapter'ı kullanacağını belirtmek için özel bir kayıt gerekebilir veya
// IAIModelAdapter'ın birden fazla implementasyonu varsa, Strategy'nin constructor'ında
// spesifik birini alması veya bir "AdapterSelector" kullanması gerekebilir.
// Şimdilik en basit haliyle kaydedelim, varsayılan bir IAIModelAdapter'ı alacağını varsayalım.
// Eğer birden fazla IAIModelAdapter varsa ve hangisinin kullanılacağı dinamikse,
// AIInitialPromptGenerationStrategy'nin kendisi bir IAIModelAdapter factory/selector alabilir.
// Veya belirli bir AI adapter'ını doğrudan enjekte alabilir:
// builder.Services.AddScoped<AIInitialPromptGenerationStrategy>(provider =>
//     new AIInitialPromptGenerationStrategy(
//         provider.GetRequiredService<OpenAIAdapter>(), // VEYA provider.GetServices<IAIModelAdapter>().FirstOrDefault(a => a.ModelIdentifier == "OpenAI-GPT-Mock")
//         provider.GetRequiredService<ILogger<AIInitialPromptGenerationStrategy>>()
//     )
// );
// Şimdilik daha genel bir kayıt yapalım, IAIModelAdapter'ın tek bir instance'ı olduğunu varsayalım
// veya AIInitialPromptGenerationStrategy'nin constructor'ı IEnumerable<IAIModelAdapter> alıp içinden seçebilir.
// En temizi, AIInitialPromptGenerationStrategy'nin constructor'ına IAIModelAdapter alması ve DI'nın bunu çözmesi.
builder.Services.AddScoped<AIInitialPromptGenerationStrategy>();








var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>(); // ApplicationUser olduğundan emin olun
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedRolesAsync(roleManager); // Helper metodu çağır
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the roles.");
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Önce Authentication
app.UseAuthorization();  // Sonra Authorization

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // Identity UI için

app.Run();
async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
{
    string[] roleNames = { "Admin", "Teacher", "Student" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            // ID'yi Identity sistemi kendi atayacak
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}