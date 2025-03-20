using Dapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly NpgsqlConnection _connection;

        public CommentRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> Create(Comment comment)
        {
            await _connection.OpenAsync();

            var sql = @"
                INSERT INTO comments (content, user_id, post_id, created_at)
                VALUES (@Content, @UserId, @PostId, @CreatedAt)
                RETURNING id
            ";
            var commentId = await _connection.QuerySingleAsync<int>(sql, new
            {
                comment.Content,
                comment.UserId,
                comment.PostId,
                comment.CreatedAt
            });

            return commentId;
        }

        public async Task<bool> Delete(int id)
        {
            await _connection.OpenAsync();

            var sql = "DELETE FROM comments WHERE id = @Id";
            var affectedRows = await _connection.ExecuteAsync(sql, new { Id = id });

            return affectedRows > 0;
        }

        public async Task<Comment> GetById(int id)
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM comments WHERE id = @Id";
            var comment = await _connection.QuerySingleOrDefaultAsync<Comment>(sql, new { Id = id });

            return comment;
        }

        public async Task<IEnumerable<Comment>> GetByUserId(int userId)
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM comments WHERE user_id = @UserId";
            var comments = await _connection.QueryAsync<Comment>(sql, new { UserId = userId });

            return comments;
        }

        public async Task<bool> Update(Comment comment)
        {
            await _connection.OpenAsync();

            var sql = @"
                UPDATE comments
                SET content = @Content,
                    user_id = @UserId,
                    post_id = @PostId,
                    created_at = @CreatedAt
                WHERE id = @Id
            ";
            var affectedRows = await _connection.ExecuteAsync(sql, new
            {
                comment.Content,
                comment.UserId,
                comment.PostId,
                comment.CreatedAt,
                comment.Id
            });

            return affectedRows > 0;
        }

        public async Task<IEnumerable<Comment>> GetAll()
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM comments";
            var comments = await _connection.QueryAsync<Comment>(sql);

            return comments;
        }

        public async Task DeleteByPostId(int postId)
        {
            await _connection.OpenAsync();

            var sql = "DELETE FROM comments WHERE post_id = @PostId";
            await _connection.ExecuteAsync(sql, new { PostId = postId });
        }

        public async Task DeleteByUserId(int userId)
        {
            await _connection.OpenAsync();

            var sql = "DELETE FROM comments WHERE user_id = @UserId";
            await _connection.ExecuteAsync(sql, new { UserId = userId });
        }
    }
}