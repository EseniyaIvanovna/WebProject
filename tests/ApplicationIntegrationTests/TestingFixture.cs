using Application;
using Bogus;
using Domain;
using Domain.Enums;
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

namespace ApplicationIntegrationTests
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

        public async Task<Post> CreatePost(int userId)
        {
            using var scope = ServiceProvider.CreateScope();
            var postRepository = scope.ServiceProvider.GetRequiredService<IPostRepository>();

            var post = new Post
            {
                UserId = userId,
                Text = _faker.Lorem.Sentence(10),
                CreatedAt = DateTime.UtcNow
            };

            post.Id = await postRepository.Create(post);
            return post;
        }

        public async Task<Comment> CreateComment(int userId, int postId)
        {
            using var scope = ServiceProvider.CreateScope();
            var commentRepository = scope.ServiceProvider.GetRequiredService<ICommentRepository>();

            var comment = new Comment
            {
                Content = _faker.Lorem.Sentence(10),
                UserId = userId,
                PostId = postId,
                CreatedAt = DateTime.UtcNow
            };

            comment.Id = await commentRepository.Create(comment);
            return comment;
        }

        public async Task<Message> CreateMessage(int senderId, int receiverId)
        {
            using var scope = ServiceProvider.CreateScope();
            var messageRepository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Text = _faker.Lorem.Sentence(10),
                CreatedAt = DateTime.UtcNow
            };

            message.Id = await messageRepository.Create(message);
            return message;
        }

        public async Task<Reaction> CreateReaction(int userId, int postId)
        {
            using var scope = ServiceProvider.CreateScope();
            var reactionRepository = scope.ServiceProvider.GetRequiredService<IReactionRepository>();

            var reaction = new Reaction
            {
                Type = ReactionType.Like,
                UserId = userId,
                PostId = postId
            };

            reaction.Id = await reactionRepository.Create(reaction);
            return reaction;
        }

        public async Task<Interaction> CreateInteraction(int user1Id, int user2Id)
        {
            using var scope = ServiceProvider.CreateScope();
            var interactionRepository = scope.ServiceProvider.GetRequiredService<IInteractionRepository>();

            var interaction = new Interaction
            {
                User1Id = user1Id,
                User2Id = user2Id,
                Status = Status.Friend
            };

            interaction.Id = await interactionRepository.Create(interaction);
            return interaction;
        }

        public async Task DisposeAsync()
        {
            await ResetDatabase();
        }

        private async Task ResetDatabase()
        {
            using var scope = ServiceProvider.CreateScope();
            var connection = scope.ServiceProvider.GetRequiredService<NpgsqlConnection>();
            await connection.OpenAsync();

            await _respawner.ResetAsync(connection);
        }
    }
}
