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
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<ICommentRepository, CommentRepository>();
            services.AddSingleton<IMessageRepository, MessageRepository>();
            services.AddSingleton<IPostRepository, PostRepository>();
            services.AddSingleton<IReactionRepository, ReactionRepository>();
            services.AddSingleton<IInteractionRepository, InteractionRepository>();

            services.AddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var configurationString = configuration.GetConnectionString("PostgresDB");
                return new NpgsqlDataSourceBuilder(configurationString).Build();
            }
            );

            services.AddScoped(sp =>
            {
                var dataSource = sp.GetRequiredService<NpgsqlDataSource>();
                return dataSource.CreateConnection();
            }
            );

            services.AddFluentMigratorCore().ConfigureRunner(
                rb => rb.AddPostgres().WithGlobalConnectionString("")
                .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole());

            return services;
        }
    }
}