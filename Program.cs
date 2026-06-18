using FirinApi.Configuration;
using FirinApi.Data;
using FirinApi.Extensions;
using FirinApi.Middleware;
using FirinApi.Models.Entities;
using FirinApi.Models.Enums;
using FirinApi.Validators.Breads;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBreadValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddHealthChecksServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    if (!await db.Breads.AnyAsync())
    {
        db.Breads.AddRange(
            new Bread { Name = "Tam Buğday", Price = 35m, StockQuantity = 50 },
            new Bread { Name = "Beyaz Ekmek", Price = 25m, StockQuantity = 80 },
            new Bread { Name = "Çavdar", Price = 40m, StockQuantity = 30 });
        await db.SaveChangesAsync();
    }

    if (!await db.Users.AnyAsync())
    {
        var seed = app.Configuration.GetSection(SeedSettings.SectionName).Get<SeedSettings>() ?? new();
        var adminPassword = seed.AdminPassword;

        if (string.IsNullOrWhiteSpace(adminPassword))
        {
            if (app.Environment.IsDevelopment())
                adminPassword = "Admin123!";
            else
                throw new InvalidOperationException("Seed:AdminPassword ortam değişkeni veya User Secrets ile tanımlanmalıdır.");
        }

        db.Users.Add(new User
        {
            Username = string.IsNullOrWhiteSpace(seed.AdminUsername) ? "admin" : seed.AdminUsername,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            Role = UserRole.Admin
        });
        await db.SaveChangesAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("FrontendPolicy"); // Frontend isteklerine izin ver (UseAuthentication'dan önce)
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthCheckEndpoint(); // GET /health → API ve DB durumu

app.Run();
