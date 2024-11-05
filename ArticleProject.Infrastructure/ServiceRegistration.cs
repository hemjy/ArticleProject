using ArticleProject.Application.Interfaces.Repositories;
using ArticleProject.Application.Interfaces.Services;
using ArticleProject.Infrastructure.Persistence.Context;
using ArticleProject.Infrastructure.Persistence.Repositories;
using ArticleProject.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using ArticleProject.Infrastructure.Persistence.Data;

namespace ArticleProject.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                   configuration.GetConnectionString("DefaultConnection"),
                   b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
            #region Repositories
            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            #endregion

            // Configure Redis
            var host = configuration.GetSection("RedisDatabaseSettings:Host").Value;
            var password = configuration.GetSection("RedisDatabaseSettings:Password").Value;
            var InstanceName = configuration.GetSection("RedisDatabaseSettings:InstanceName").Value;
            var port = configuration.GetSection("RedisDatabaseSettings:Port").Value;

            var redisConnectionString = $"{host}:{port}";
            if (!string.IsNullOrWhiteSpace(password)) redisConnectionString = $"{redisConnectionString},password={password}";
            var redis = ConnectionMultiplexer.Connect(redisConnectionString);

            services.AddSingleton<IConnectionMultiplexer>(redis);
            services.AddScoped<IDistributedLockService, RedisDistributedLockService>();

            // Seed data after the context is configured
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            DataSeeder.Seed(context);
        }

    }
}
