using Logging.Application.DTOs;
using Logging.Domain.Entities;
using Logging.Domain.IRepositories;
using Logging.Infrastructure.Contexts;
using Logging.Infrastructure.Repositories;
using Logging.Infrastructure.Services;
using LoggingService.Application.IServices;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Logging database connection string is missing.");

builder.Services.AddDbContext<LoggingDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ILogService, LogService>();

// FIXED: Cleaned up AutoMapper declaration to avoid assembly scanning conflicts
builder.Services.AddAutoMapper(cfg =>
{
    cfg.CreateMap<Log, LogDto>();
    cfg.CreateMap<CreateLogDto, Log>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LoggingDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} 

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();