using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using LibrarySystem.Application;
using LibrarySystem.DataAccess;
using LibrarySystem.DataAccess.Persistence.Contexts;
using LibrarySystem.DataAccess.Persistence.models;
using LibrarySystem.DataAccess.Persistence.Seeds;
using LibrarySystem.API.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// ── CORS ───────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // React dev server
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key is missing.")))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Error("[JWT Auth Error] Failed validating token. Reason: {Error}",
                context.Exception.Message);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter a valid token. Format: eyJhbG..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    Log.Information("Applying application database migrations.");
    var applicationDbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();
    await applicationDbContext.Database.MigrateAsync();

    Log.Information("Applying identity database migrations.");
    var identityDbContext = scope.ServiceProvider
        .GetRequiredService<CustomIdentityDbContext>();
    await identityDbContext.Database.MigrateAsync();

    Log.Information("Seeding identity roles.");
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<AppRoleModel>>();
    await RoleSeeder.SeedRolesAsync(roleManager);
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStaticFiles();
app.UseRouting();

// ── Order matters here ─────────────────────────────────────────────
app.UseCors("AllowReactApp");        // ← must come before Authentication
app.UseAuthentication();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();