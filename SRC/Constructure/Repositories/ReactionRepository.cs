using Dapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            await _connection.OpenAsync();

            var sql = @"
                INSERT INTO reactions (type, userid, postid)
                VALUES (@Type, @UserId, @PostId)
                RETURNING id;
            ";

            var reactionId = await _connection.QuerySingleAsync<int>(sql, new
            {
                reaction.Type,
                reaction.UserId,
                reaction.PostId
            });

            return reactionId;
        }

        public async Task<bool> Delete(int id)
        {
            await _connection.OpenAsync();

            var sql = "DELETE FROM reactions WHERE id = @Id";
            var affectedRows = await _connection.ExecuteAsync(sql, new { Id = id });

            return affectedRows > 0;
        }

        public async Task<Reaction> GetById(int id)
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM reactions WHERE id = @Id";
            var reaction = await _connection.QuerySingleOrDefaultAsync<Reaction>(sql, new { Id = id });

            return reaction;
        }

        public async Task<IEnumerable<Reaction>> GetByPostId(int postId)
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM reactions WHERE postid = @PostId";
            var reactions = await _connection.QueryAsync<Reaction>(sql, new { PostId = postId });

            return reactions;
        }

        public async Task<IEnumerable<Reaction>> GetByUserId(int userId)
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM reactions WHERE userid = @UserId";
            var reactions = await _connection.QueryAsync<Reaction>(sql, new { UserId = userId });

            return reactions;
        }

        public async Task<bool> Update(Reaction reaction)
        {
            await _connection.OpenAsync();

            var sql = @"
                UPDATE reactions
                SET type = @Type,
                    userid = @UserId,
                    postid = @PostId
                WHERE id = @Id;
            ";

            var affectedRows = await _connection.ExecuteAsync(sql, new
            {
                reaction.Type,
                reaction.UserId,
                reaction.PostId,
                reaction.Id
            });

            return affectedRows > 0;
        }

        public async Task<IEnumerable<Reaction>> GetAll()
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM reactions";
            var reactions = await _connection.QueryAsync<Reaction>(sql);

            return reactions;
        }

        public async Task DeleteByPostId(int postId)
        {
            await _connection.OpenAsync();

            var sql = "DELETE FROM reactions WHERE postid = @PostId";
            await _connection.ExecuteAsync(sql, new { PostId = postId });
        }

        public async Task DeleteByUserId(int userId)
        {
            await _connection.OpenAsync();

            var sql = "DELETE FROM reactions WHERE userid = @UserId";
            await _connection.ExecuteAsync(sql, new { UserId = userId });
        }
    }
}