﻿using Dapper;
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
                INSERT INTO comments (postId, userId, content, createdAt)
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

        public async Task Delete(int id)
        {
            var sql = "DELETE FROM comments WHERE id = @Id";
            await _connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<Comment?> GetById(int id)
        {
            var sql = @"
                SELECT id, postId, userId, content, createdAt
                FROM comments
                WHERE id = @Id;
            ";

            var comment = await _connection.QuerySingleOrDefaultAsync<Comment>(sql, new { Id = id });
            return comment;
        }

        public async Task<IEnumerable<Comment>> GetByUserId(int userId)
        {
            var sql = @"
                SELECT id, postId, userId, content, createdAt
                FROM comments
                WHERE userId = @UserId;
            ";
            var comments = await _connection.QueryAsync<Comment>(sql, new { UserId = userId });

            return comments;
        }

        public async Task Update(Comment comment)
        {
            var sql = @"
                UPDATE comments
                SET content = @Content
                WHERE id = @Id;
            ";

             await _connection.ExecuteAsync(sql, new
            {
                comment.Content,
                comment.Id
            });
        }

        public async Task<IEnumerable<Comment>> GetAll()
        {
            var sql = @"
                SELECT id, postId, userId, content, createdAt
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