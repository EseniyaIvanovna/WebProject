using Dapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Npgsql;

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
            comment.CreatedAt = DateTime.UtcNow;

            var sql = @"
                INSERT INTO comments (post_id, user_id, content, created_at)
                VALUES (@PostId, @UserId, @Content, @CreatedAt)
                RETURNING id;
            ";

            var commentId = await _connection.QuerySingleAsync<int>(sql, new
            {
                comment.PostId,
                comment.UserId,
                comment.Content,
                comment.CreatedAt
            });

            return commentId;
        }

        public async Task<bool> Delete(int id)
        {
            var sql = "DELETE FROM comments WHERE id = @Id";
            var affectedRows = await _connection.ExecuteAsync(sql, new { Id = id });

            return affectedRows > 0;
        }
        public async Task DeleteByPostOwnerId(int userId)
        {
            await _connection.ExecuteAsync(
            @"DELETE FROM comments 
              WHERE post_id IN (SELECT id FROM posts WHERE user_id = @UserId)",
            new { UserId = userId });
        }

        public async Task<Comment?> GetById(int id)
        {
            var sql = @"
                SELECT id, post_id, user_id, content, created_at
                FROM comments
                WHERE id = @Id;
            ";

            var comment = await _connection.QuerySingleOrDefaultAsync<Comment>(sql, new { Id = id });
            return comment;
        }

        public async Task<IEnumerable<Comment>> GetByUserId(int userId)
        {
            var sql = @"
                SELECT id, post_id, user_id, content, created_at
                FROM comments
                WHERE user_id = @UserId;
            ";
            var comments = await _connection.QueryAsync<Comment>(sql, new { UserId = userId });

            return comments;
        }

        public async Task<bool> Update(Comment comment)
        {
            var sql = @"
                UPDATE comments
                SET content = @Content
                WHERE id = @Id;
            ";

            var affectedRows = await _connection.ExecuteAsync(sql, new
             {
                 comment.Content,
                 comment.Id
             });

            return affectedRows > 0;
        }

        public async Task<IEnumerable<Comment>> GetAll()
        {
            var sql = @"
                SELECT id, post_id, user_id, content, created_at
                FROM comments;
            ";

            var comments = await _connection.QueryAsync<Comment>(sql);
            return comments;
        }

        public async Task DeleteByPostId(int postId)
        {
            var sql = "DELETE FROM comments WHERE postId = @PostId";
            await _connection.ExecuteAsync(sql, new { PostId = postId });
        }

        public async Task DeleteByUserId(int userId)
        {
            await _connection.ExecuteAsync(
                "DELETE FROM comments WHERE userId = @UserId",
                new { UserId = userId });
        }
    }
}