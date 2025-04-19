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
        private IHost _host;

        public TestingFixture()
        {
            _faker = new Faker();

            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("PostgresDBIntegration");

                    if (string.IsNullOrWhiteSpace(connectionString))
                        throw new ApplicationException("Database connection string is not configured");

                    services.AddInfrastructure();
                    services.AddApplication();

                    services.AddSingleton(_ => new NpgsqlDataSourceBuilder(connectionString).Build());

                    services.AddTransient(sp =>
                    {
                        var dataSource = sp.GetRequiredService<NpgsqlDataSource>();
                        var connection = dataSource.CreateConnection();
                        connection.Open();
                        return connection;
                    });

                    services
                        .AddFluentMigratorCore()
                        .ConfigureRunner(rb => rb
                            .AddPostgres()
                            .WithGlobalConnectionString(connectionString)
                            .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
                        .Configure<SelectingProcessorAccessorOptions>(options =>
                        {
                            options.ProcessorId = "Postgres";
                        });
                })
                .Build();

            ServiceProvider = _host.Services;
        }

        public IServiceProvider ServiceProvider { get; }

        public async Task InitializeAsync()
        {
            try
            {
                using var scope = ServiceProvider.CreateScope();
                var connection = scope.ServiceProvider.GetRequiredService<NpgsqlConnection>();

                var migrationRunner = scope.ServiceProvider.GetRequiredService<MigrationRunner>();
                migrationRunner.Run();

                _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
                {
                    DbAdapter = DbAdapter.Postgres,
                    SchemasToInclude = ["public"],
                    TablesToIgnore = ["VersionInfo"]
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialize test fixture", ex);
            }
        }

        public async Task DisposeAsync()
        {
            try
            {
                if (_respawner != null)
                {
                    using var scope = ServiceProvider.CreateScope();
                    var connection = scope.ServiceProvider.GetRequiredService<NpgsqlConnection>();

                    if (connection.State != System.Data.ConnectionState.Open)
                        await connection.OpenAsync();

                    await _respawner.ResetAsync(connection);
                    await connection.CloseAsync();
                }
            }
            finally
            {
                if (_host != null)
                {
                    await _host.StopAsync();
                    _host.Dispose();
                }
            }
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