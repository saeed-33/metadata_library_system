using LibrarySystem.Application.Interfaces;
using LibrarySystem.DataAccess.Persistence.Contexts;
using LibrarySystem.DataAccess.Persistence.models;
using LibrarySystem.DataAccess.Repositories;
using LibrarySystem.DataAccess.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LibrarySystem.DataAccess.Services;

namespace LibrarySystem.DataAccess
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var defaultConnection = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection is missing.");

            var identityConnection = configuration.GetConnectionString("IdentityConnection")
                ?? configuration.GetConnectionString("IdentifyConnection")
                ?? throw new InvalidOperationException("IdentityConnection is missing.");
           
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(defaultConnection));

            services.AddDbContext<CustomIdentityDbContext>(options =>
                options.UseSqlServer(identityConnection));

            services.AddIdentity<AppUserModel, AppRoleModel>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<CustomIdentityDbContext>()
            .AddDefaultTokenProviders();

            services.AddAutoMapper(config =>
            {
                config.AddProfile<Mappings.PersistenceMappingProfile>();
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthService, AuthService>(); // ← after Identity
            services.AddHttpClient<ILoggingClient, LoggingClient>(client =>
            {
                var baseUrl = configuration["LoggingService:BaseUrl"] ?? "http://localhost:5113";
                client.BaseAddress = new Uri(baseUrl);
            });

            return services;
        }
    }
}
