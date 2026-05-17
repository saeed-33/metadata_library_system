using LibrarySystem.Application.Interfaces;
using LibrarySystem.DataAccess.Persistence.Contexts;
using LibrarySystem.DataAccess.Persistence.models;
using LibrarySystem.DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            var identityConnection = configuration.GetConnectionString("IdentifyConnection")
                ?? throw new InvalidOperationException("IdentifyConnection is missing.");

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

            return services;
        }
    }
}
