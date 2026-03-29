using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MyContainerApp.Domain.Repositories;
using MyContainerApp.Infrastructure.Persistence;
using MyContainerApp.Infrastructure.Persistence.Repositories;
using MyContainerApp.Application.Mappings;
using MyContainerApp.Application.Services;

namespace MyContainerApp.Infrastructure;

/// <summary>
/// Extension methods for dependency injection and service registration.
/// Configures all infrastructure services including EF Core, repositories, and AutoMapper.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds infrastructure services to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Register Entity Framework Core with in-memory database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("PizzaDb"));

        // Register repositories
        services.AddScoped<IPizzaRepository, PizzaRepository>();

        // Register application services
        services.AddScoped<IPizzaApplicationService, PizzaApplicationService>();

        // Register AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        return services;
    }
}
