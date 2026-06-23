using FirinApi.Data;
using FirinApi.Repositories.Implementations;
using FirinApi.Repositories.Interfaces;
using FirinApi.Services.Implementations;
using FirinApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FirinApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IBreadRepository, BreadRepository>();
        services.AddScoped<ICakeRepository, CakeRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IStandingOrderRepository, StandingOrderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IBreadService, BreadService>();
        services.AddScoped<ICakeService, CakeService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IStandingOrderService, StandingOrderService>();
        services.AddScoped<IDeliveryService, DeliveryService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
