﻿using Dapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Npgsql;

namespace Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly NpgsqlConnection _connection;

        public PostRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> Create(Post post)
        {
            post.CreatedAt = DateTime.UtcNow;

            var sql = @"
                INSERT INTO posts (user_id, text, created_at)
                VALUES (@UserId, @Text, @CreatedAt)
                RETURNING id;
            ";

            var postId = await _connection.QuerySingleAsync<int>(sql, new
            {
                post.UserId,
                post.Text,
                post.CreatedAt
            });

            return postId;
        }

        public async Task<bool> Delete(int id)
        {
            var sql = "DELETE FROM posts WHERE id = @Id";
            var affectedRows = await _connection.ExecuteAsync(sql, new { Id = id });

            return affectedRows > 0;
        }

        public async Task DeleteByUserId(int userId)
        {
            await _connection.ExecuteAsync(
                "DELETE FROM posts WHERE userId = @UserId",
                new { UserId = userId });
        }

        public async Task<IEnumerable<Post>> GetAll()
        {
            var sql = @"
                SELECT id, user_id, text, created_at
                FROM posts;
            ";

            var posts = await _connection.QueryAsync<Post>(sql);
            return posts;
        }

        public async Task<Post?> GetById(int id)
        {
            var sql = @"
                SELECT id, user_id, text, created_at
                FROM posts
                WHERE id = @Id;
            ";

            var post = await _connection.QuerySingleOrDefaultAsync<Post>(sql, new { Id = id });
            return post;
        }

        public async Task<bool> Update(Post post)
        {
            var sql = @"
                UPDATE posts
                SET text = @Text
                WHERE id = @Id;
            ";

            var affectedRows = await _connection.ExecuteAsync(sql, new
            {
                post.Text,
                post.Id
            });

            return affectedRows > 0;
        }
        public async Task<IEnumerable<Post>> GetByUserId(int userId)
        {
            var sql = @"SELECT id, user_id, text, ""created_at""
                FROM posts WHERE user_id = @UserId";
            var posts = await _connection.QueryAsync<Post>(sql, new { UserId = userId });

            return posts;
        }
    }
}