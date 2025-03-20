using Dapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            await _connection.OpenAsync();

            var sql = @"
                INSERT INTO posts (user_id, text, created_at)
                VALUES (@UserId, @Text, @CreatedAt)
                RETURNING id
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
            await _connection.OpenAsync();

            var sql = "DELETE FROM posts WHERE id = @Id";
            var affectedRows = await _connection.ExecuteAsync(sql, new { Id = id });

            return affectedRows > 0;
        }

        public async Task DeleteByUserId(int userId)
        {
            await _connection.OpenAsync();

            var sql = "DELETE FROM posts WHERE user_id = @UserId";
            await _connection.ExecuteAsync(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<Post>> GetAll()
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM posts";
            var posts = await _connection.QueryAsync<Post>(sql);

            return posts;
        }

        public async Task<Post> GetById(int id)
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM posts WHERE id = @Id";
            var post = await _connection.QuerySingleOrDefaultAsync<Post>(sql, new { Id = id });

            return post;
        }

        public async Task<bool> Update(Post post)
        {
            await _connection.OpenAsync();

            var sql = @"
                UPDATE posts
                SET text = @Text,
                    user_id = @UserId,
                    created_at = @CreatedAt
                WHERE id = @Id
            ";
            var affectedRows = await _connection.ExecuteAsync(sql, new
            {
                post.Text,
                post.UserId,
                post.CreatedAt,
                post.Id
            });

            return affectedRows > 0;
        }
    }
}