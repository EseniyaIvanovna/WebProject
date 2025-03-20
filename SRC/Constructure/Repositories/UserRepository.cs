using Dapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly NpgsqlConnection _connection;

        public UserRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> Create(User user)
        {
            await _connection.OpenAsync();

            var sql = @"
                INSERT INTO users (name, lastname, email, age, info)
                VALUES (@Name, @LastName, @Email, @Age, @Info)
                RETURNING id
            ";
            var userId = await _connection.QuerySingleAsync<int>(sql, new
            {
                user.Name,
                user.LastName,
                user.Email,
                user.Age,
                user.Info
            });

            return userId;
        }

        public async Task<bool> Delete(int id)
        {
            await _connection.OpenAsync();

            var sql = "DELETE FROM users WHERE id = @Id";
            var affectedRows = await _connection.ExecuteAsync(sql, new { Id = id });

            return affectedRows > 0;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM users";
            var users = await _connection.QueryAsync<User>(sql);

            return users;
        }

        public async Task<User> GetById(int id)
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM users WHERE id = @Id";
            var user = await _connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });

            return user;
        }

        public async Task<bool> Update(User user)
        {
            await _connection.OpenAsync();

            var sql = @"
                UPDATE users
                SET name = @Name,
                    lastname = @LastName,
                    email = @Email,
                    age = @Age,
                    info = @Info
                WHERE id = @Id
            ";
            var affectedRows = await _connection.ExecuteAsync(sql, new
            {
                user.Name,
                user.LastName,
                user.Email,
                user.Age,
                user.Info,
                user.Id
            });

            return affectedRows > 0;
        }
    }
}