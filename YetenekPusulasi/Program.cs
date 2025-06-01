using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces;
using YetenekPusulasi.Core.Interfaces.Repositories;
using YetenekPusulasi.Core.Interfaces.Observers;
using YetenekPusulasi.Core.Services;
using YetenekPusulasi.Infrastructure.Data;
using YetenekPusulasi.Infrastructure.Repositories;
using YetenekPusulasi.Infrastructure.Observers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// Register repositories
builder.Services.AddScoped<IClassroomRepository, ClassroomRepository>();
builder.Services.AddScoped<IRepository<ApplicationUser>, Repository<ApplicationUser>>();
builder.Services.AddScoped<IRepository<Classroom>, Repository<Classroom>>();
builder.Services.AddScoped<IRepository<StudentClassroom>, Repository<StudentClassroom>>();
builder.Services.AddScoped<IRepository<Scenario>, Repository<Scenario>>();
builder.Services.AddScoped<IRepository<ScenarioQuestion>, Repository<ScenarioQuestion>>();
builder.Services.AddScoped<IRepository<StudentScenario>, Repository<StudentScenario>>();
builder.Services.AddScoped<IRepository<StudentScenarioAnswer>, Repository<StudentScenarioAnswer>>();

// Register services
builder.Services.AddScoped<ClassroomManagementService>();
builder.Services.AddScoped<StudentEnrollmentFacade>();

// Register observers
builder.Services.AddScoped<IClassroomObserver, ClassroomObserver>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run(); 