using Microsoft.Extensions.DependencyInjection;
using Store.Application.Interfaces;
using Store.Database.Postgres.Repositories;


namespace Store.Database.Postgres.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgresRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}