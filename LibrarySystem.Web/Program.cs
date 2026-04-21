using Microsoft.EntityFrameworkCore;
using LibrarySystem.DataAccess.Persistence.Contexts;
using Serilog;
using LibrarySystem.DataAccess.Persistence.models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);




Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 1. Get Connection String
var connectionString = builder.Configuration.GetConnectionString("IdentifyConnection");

// 2. Register DbContext
builder.Services.AddDbContext<CustomIdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Register Identity Services
builder.Services.AddIdentity<AppUserModel, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<CustomIdentityDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<LibrarySystem.Application.Mappings.VocabularyMappingProfile>();
});
// Add services to the container.
builder.Services.AddControllersWithViews();

// Unit of Work (replaces individual IGenericRepository registrations)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register MediatR and scan all handlers in Application layer
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(LibrarySystem.Application.Commands.CreateVocabularyCommand).Assembly
    ));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
