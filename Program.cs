// Program.cs (kısaltılmış, sadece eklenenler)
using YetenekPusulasi.Core.Interfaces.Repositories;
using YetenekPusulasi.Core.Interfaces.Strategies;
using YetenekPusulasi.Core.Interfaces.Services;
using YetenekPusulasi.Core.Services;
using YetenekPusulasi.Core.Strategies;
using YetenekPusulasi.Data; // ApplicationDbContext için
using YetenekPusulasi.Data.Repositories;
using Microsoft.EntityFrameworkCore; // EF Core için
using Microsoft.AspNetCore.Identity.Data;
using YetenekPusulasi.Models;
using Microsoft.AspNetCore.Identity; // Identity için
// using Microsoft.AspNetCore.Authentication.JwtBearer; // API için JWT kullanacaksanız
// using Microsoft.IdentityModel.Tokens; // JWT için
// using System.Text; // JWT için
using YetenekPusulasi.Core.Interfaces.Repositories;


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
// builder.Services.AddRazorPages(); // Eğer Razor Pages kullanacaksanız

// --- Bizim Eklediğimiz Servisler ---
// Repository'ler
builder.Services.AddScoped<IScenarioRepository, ScenarioRepository>();
builder.Services.AddScoped<IScenarioCategoryRepository, ScenarioCategoryRepository>();
builder.Services.AddScoped<YetenekPusulasi.Core.Interfaces.Services.IScenarioService, YetenekPusulasi.Core.Services.ScenarioService>();
// Stratejiler (tümünü kaydedip Service içinde seçiyoruz)
builder.Services.AddScoped<RuleBasedScenarioStrategy>(); // Somut tip olarak kaydet
builder.Services.AddScoped<AIModelScenarioStrategy>();   // Somut tip olarak kaydet
// IEnumerable<IScenarioGenerationStrategy> olarak enjekte etmek için:
builder.Services.AddScoped<IScenarioGenerationStrategy, RuleBasedScenarioStrategy>(); // Biri IScenarioGenerationStrategy olarak kaydedilmeli
builder.Services.AddScoped<IScenarioGenerationStrategy, AIModelScenarioStrategy>();   // Diğeri de IScenarioGenerationStrategy olarak kaydedilmeli
// Not: Aynı arayüz için birden fazla implementasyon kaydettiğinizde,
// ScenarioService'in constructor'ında IEnumerable<IScenarioGenerationStrategy> alarak hepsini enjekte edebilirsiniz.

// Servis Katmanı
builder.Services.AddScoped<IScenarioService, ScenarioService>();

// --- API için JWT Authentication (Opsiyonel) ---
/*
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Development için false, production için true
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});
*/
// appsettings.json'a JWT ayarları eklenmeli:
/*
"JWT": {
  "ValidAudience": "http://localhost:yourport",
  "ValidIssuer": "http://localhost:yourport",
  "Secret": "THIS IS USED TO SIGN AND VERIFY JWT TOKENS, REPLACE IT WITH YOUR OWN SECRET" // Güçlü bir anahtar olmalı
}
*/


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