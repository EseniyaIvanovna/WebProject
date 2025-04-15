using Dapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Npgsql;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly NpgsqlConnection _connection;

        public UserRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> Create(User user) //добавить пароль и роль
        {
            var sql = @"
                INSERT INTO users (name, lastname, dateOfBirth, info, email)
                VALUES (@Name, @LastName, @DateOfBirth, @Info, @Email)
                RETURNING id;
            ";

            var userId = await _connection.QuerySingleAsync(sql, user);

            return userId;
        }

        public async Task<bool> Delete(int id)
        {
            var sql = "DELETE FROM users WHERE id = @Id";
            var affectedRows = await _connection.ExecuteAsync(sql, new { Id = id });

            return affectedRows > 0;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            var sql = @"
                SELECT id, name, lastname, dateOfBirth, info, email
                FROM users;
            ";

            var users = await _connection.QueryAsync<User>(sql);
            return users;
        }

        public async Task<User> GetById(int id)
        {
            var sql = @"
                SELECT id, name, lastname, dateOfBirth, info, email
                FROM users
                WHERE id = @Id;
            ";

            var user = await _connection.QuerySingleAsync<User>(sql, new { Id = id });
            return user;
        }

        public async Task<bool> Update(User user)
        {
            var sql = @"
                UPDATE users
                SET name = @Name,
                    lastname = @LastName,
                    dateOfBirth = @DateOfBirth,
                    info = @Info,
                    email = @Email
                WHERE id = @Id;
            ";

            var affectedRows = await _connection.ExecuteAsync(sql, new
            {
                user.Name,
                user.LastName,
                user.DateOfBirth,
                user.Info,
                user.Email,
                user.Id
            });

            return affectedRows > 0;
        }
    }
}