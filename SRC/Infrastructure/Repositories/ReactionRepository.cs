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
                INSERT INTO reactions (user_id, post_id, type)
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

        public async Task<bool> Delete(int id)
        {
            var sql = "DELETE FROM reactions WHERE id = @Id";
            var affectedRows = await _connection.ExecuteAsync(sql, new { Id = id });

            return affectedRows > 0;
        }

        public async Task<Reaction?> GetById(int id)
        {
            var sql = @"
                SELECT id, user_id, post_id, type as Type
                FROM reactions
                WHERE id = @Id;
            ";

            var reaction = await _connection.QuerySingleOrDefaultAsync<Reaction>(sql, new { Id = id });
            return reaction;
        }

        public async Task<IEnumerable<Reaction>> GetByPostId(int postId)
        {
            var sql = @"
                SELECT id, user_id, post_id, type as Type
                FROM reactions
                WHERE post_id = @PostId;
            ";

            var reactions = await _connection.QueryAsync<Reaction>(sql, new { PostId = postId });
            return reactions;
        }

        public async Task<IEnumerable<Reaction>> GetByUserId(int userId)
        {
            var sql = @"
                SELECT id, user_id, post_id, type as Type
                FROM reactions
                WHERE user_id = @UserId;
            ";
            var reactions = await _connection.QueryAsync<Reaction>(sql, new { UserId = userId });

            return reactions;
        }

        public async Task<bool> Update(Reaction reaction)
        {
            var sql = @"
                UPDATE reactions
                SET type = @Type
                WHERE id = @Id;
            ";

            var affectedRows = await _connection.ExecuteAsync(sql, new
            {
                reaction.Type,
                reaction.Id
            });

            return affectedRows > 0;
        }

        public async Task<IEnumerable<Reaction>> GetAll()
        {
            var sql = @"
                SELECT id, user_id, post_id, type as Type
                FROM reactions;
            ";

            var reactions = await _connection.QueryAsync<Reaction>(sql);

            return reactions;
        }

        public async Task DeleteByPostId(int postId)
        {
            var sql = @"DELETE FROM reactions WHERE post_id = @PostId";
            await _connection.ExecuteAsync(sql, new { PostId = postId });
        }

        public async Task DeleteByUserId(int userId)
        {
            await _connection.ExecuteAsync(
                @"DELETE FROM reactions WHERE user_id = @UserId",
                new { UserId = userId });
        }

        public async Task DeleteByPostOwnerId(int userId)
        {
            await _connection.ExecuteAsync(
            @"DELETE FROM reactions 
              WHERE post_id IN (SELECT id FROM posts WHERE user_id = @UserId)",
            new { UserId = userId });
        }

        public async Task<bool> Exists(int userId, int postId)
        {
            var sql = @"SELECT EXISTS(SELECT 1 FROM reactions WHERE user_id = @UserId AND post_id = @PostId)";
            return await _connection.QuerySingleAsync<bool>(sql, new { UserId = userId, PostId = postId });
        }
    }
}