using FluentMigrator.Runner;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Reflection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IReactionRepository, ReactionRepository>();
            services.AddScoped<IInteractionRepository, InteractionRepository>();

            services.AddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("PostgresDB");
                return new NpgsqlDataSourceBuilder(connectionString).Build();
            });

            services.AddScoped(sp =>
            {
                var dataSource = sp.GetRequiredService<NpgsqlDataSource>();
                return dataSource.CreateConnection();
            });

            services.AddFluentMigratorCore()
                    .ConfigureRunner(rb => rb.AddPostgres()
                    .WithGlobalConnectionString("PostgresDB")
                    .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
                    .AddLogging(lb => lb.AddFluentMigratorConsole());
            services.AddScoped<Database.MigrationRunner>();

            return services;
        }
    }
}