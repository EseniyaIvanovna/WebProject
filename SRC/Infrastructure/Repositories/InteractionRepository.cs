using Dapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Npgsql;

namespace Infrastructure.Repositories
{
    public class InteractionRepository : IInteractionRepository
    {
        private readonly NpgsqlConnection _connection;

        public InteractionRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> Create(Interaction interaction)
        {
            var sql = @"
                INSERT INTO interactions (user1Id, user2Id, status)
                VALUES (@User1Id, @User2Id, @Status)
                RETURNING id;
            ";

            var interactionId = await _connection.QuerySingleAsync<int>(sql, new
            {
                interaction.User1Id,
                interaction.User2Id,
                Status = interaction.Status.ToString()
            });

            return interactionId;
        }

        public async Task Delete(int id)
        {
            var sql = "DELETE FROM interactions WHERE id = @Id";
            await _connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<Interaction?> GetById(int id)
        {
            var sql = @"
                SELECT id, user1Id, user2Id, status
                FROM interactions
                WHERE id = @Id;
            ";

            var interaction = await _connection.QuerySingleOrDefaultAsync<Interaction>(sql, new { Id = id });
            return interaction;
        }

        public async Task<IEnumerable<Interaction>> GetByStatus(Status status)
        {
            var sql = @"
                SELECT id, user1Id, user2Id, status
                FROM interactions
                WHERE status = @Status;
            ";

            var interactions = await _connection.QueryAsync<Interaction>(sql, new { Status = status.ToString() });
            return interactions;
        }

        public async Task Update(Interaction interaction)
        {
            var sql = @"
                UPDATE interactions
                SET user1Id = @User1Id,
                    user2Id = @User2Id,
                    status = @Status
                WHERE id = @Id;
            ";

            await _connection.ExecuteAsync(sql, new
            {
                interaction.User1Id,
                interaction.User2Id,
                Status = interaction.Status.ToString(), 
                interaction.Id
            });
        }

        public async Task<IEnumerable<Interaction>> GetAll()
        {
            var sql = @"
                SELECT id, user1Id, user2Id, status
                FROM interactions;
            ";

            var interactions = await _connection.QueryAsync<Interaction>(sql);

            return interactions;
        }

        public async Task<bool> ExistsBetweenUsers(int user1Id, int user2Id)
        {
            var sql = @"SELECT EXISTS(
                SELECT 1 FROM interactions 
                WHERE (user1id = @User1Id AND user2id = @User2Id)
                OR (user1id = @User2Id AND user2id = @User1Id)
            )";
            return await _connection.QuerySingleAsync<bool>(sql, new
            {
                User1Id = user1Id,
                User2Id = user2Id
            });
        }

        public async Task DeleteByUserId(int userId)
        {
            var sql = "DELETE FROM interactions WHERE user1id = @UserId OR user2id = @UserId";
            await _connection.ExecuteAsync(sql, new { UserId = userId });
        }
    }
}