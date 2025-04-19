using Application;
using Bogus;
using Domain;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using Infrastructure;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Respawn;
using System.Reflection;
using MigrationRunner = Infrastructure.Database.MigrationRunner;

namespace InfrastructureIntegrationTests
{
    public sealed class TestingFixture : IAsyncLifetime
    {
        private readonly Faker _faker;
        private Respawner _respawner = null!;

        public TestingFixture()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) => { config.AddJsonFile("appsettings.json"); })
                .ConfigureServices((context, services) =>
                {
                    services.AddInfrastructure();
                    services.AddApplication();
                    var connectionString = context.Configuration.GetConnectionString("PostgresDBIntegration");
                    if (string.IsNullOrWhiteSpace(connectionString))
                        throw new ApplicationException("PostgresDBIntegration connection string is empty");

                    services.AddSingleton(_ => new NpgsqlDataSourceBuilder(connectionString).Build());

                    services.AddTransient(sp =>
                    {
                        var dataSource = sp.GetRequiredService<NpgsqlDataSource>();
                        return dataSource.CreateConnection();
                    });

                    services
                        .AddFluentMigratorCore()
                        .ConfigureRunner(rb => rb
                            .AddPostgres()
                            .WithGlobalConnectionString(connectionString)
                            .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
                        .Configure<SelectingProcessorAccessorOptions>(options => { options.ProcessorId = "Postgres"; });
                })
                .Build();

            ServiceProvider = host.Services;
            _faker = new Faker();
        }

        public IServiceProvider ServiceProvider { get; }

        public async Task InitializeAsync()
        {
            using var scope = ServiceProvider.CreateScope();
            var connection = scope.ServiceProvider.GetRequiredService<NpgsqlConnection>();
            await connection.OpenAsync();

            var migrationRunner = scope.ServiceProvider.GetRequiredService<MigrationRunner>();
            migrationRunner.Run();

            _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"],
                TablesToIgnore = ["VersionInfo"]
            });
        }

        public async Task DisposeAsync()
        {
            using var scope = ServiceProvider.CreateScope();
            var connection = scope.ServiceProvider.GetRequiredService<NpgsqlConnection>();
            await connection.OpenAsync();
            await _respawner.ResetAsync(connection);
        }

        public async Task<User> CreateUser()
        {
            using var scope = ServiceProvider.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var user = new User
            {
                Name = _faker.Name.FirstName(),
                LastName = _faker.Name.LastName(),
                DateOfBirth = _faker.Date.Past(),
                Info = _faker.Lorem.Sentence(10),
                Email = _faker.Internet.Email()
            };

            user.Id = await userRepository.Create(user);
            return user;
        }
    }
}