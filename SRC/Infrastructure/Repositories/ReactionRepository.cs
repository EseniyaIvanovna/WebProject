using Dapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Npgsql;

namespace Infrastructure.Repositories
{
    public class ReactionRepository : IReactionRepository
    {
        private readonly NpgsqlConnection _connection;

        public ReactionRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> Create(Reaction reaction)
        {
            var sql = @"
                INSERT INTO reactions (userId, postId, type)
                VALUES (@UserId, @PostId, @Type)
                RETURNING id;
            ";

            var reactionId = await _connection.QuerySingleAsync<int>(sql, new
            {
                reaction.UserId,
                reaction.PostId,
                reaction.Type
            });

            return reactionId;
        }

        public async Task Delete(int id)
        {
            var sql = "DELETE FROM reactions WHERE id = @Id";
            await _connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<Reaction?> GetById(int id)
        {
            var sql = @"
                SELECT id, userId, postId, type
                FROM reactions
                WHERE id = @Id;
            ";

            var reaction = await _connection.QuerySingleOrDefaultAsync<Reaction>(sql, new { Id = id });
            return reaction;
        }

        public async Task<IEnumerable<Reaction>> GetByPostId(int postId)
        {
            var sql = @"
                SELECT id, userId, postId, type
                FROM reactions
                WHERE postId = @PostId;
            ";
            var reactions = await _connection.QueryAsync<Reaction>(sql, new { PostId = postId });

            return reactions;
        }

        public async Task<IEnumerable<Reaction>> GetByUserId(int userId)
        {
            var sql = @"
                SELECT id, userId, postId, type
                FROM reactions
                WHERE userId = @UserId;
            ";
            var reactions = await _connection.QueryAsync<Reaction>(sql, new { UserId = userId });

            return reactions;
        }

        public async Task Update(Reaction reaction)
        {
            var sql = @"
                UPDATE reactions
                SET type = @Type
                WHERE id = @Id;
            ";

            await _connection.ExecuteAsync(sql, new
            {
                reaction.Type,
                reaction.Id
            });
        }

        public async Task<IEnumerable<Reaction>> GetAll()
        {
            var sql = @"
                SELECT id, userId, postId, type
                FROM reactions;
            ";

            var reactions = await _connection.QueryAsync<Reaction>(sql);

            return reactions;
        }

        public async Task DeleteByPostId(int postId)
        {
            var sql = "DELETE FROM reactions WHERE postId = @PostId";
            await _connection.ExecuteAsync(sql, new { PostId = postId });
        }

        public async Task DeleteByUserId(int userId, NpgsqlTransaction transaction)
        {
            await _connection.ExecuteAsync(
                "DELETE FROM reactions WHERE userId = @UserId",
                new { UserId = userId },
                transaction: transaction);
        }
        public async Task DeleteByPostOwnerId(int userId, NpgsqlTransaction transaction)
        {
            await _connection.ExecuteAsync(
            @"DELETE FROM reactions 
              WHERE postId IN (SELECT id FROM posts WHERE userId = @UserId)",
            new { UserId = userId },
            transaction: transaction);
        }

        public async Task<bool> Exists(int userId, int postId)
        {
            var sql = "SELECT EXISTS(SELECT 1 FROM reactions WHERE userid = @UserId AND postid = @PostId)";
            return await _connection.QuerySingleAsync<bool>(sql, new { UserId = userId, PostId = postId });
        }
    }
}