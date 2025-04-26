using Dapper;
using Domain;
using Infrastructure.Database.TypeMappings;
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

        public async Task<int> Create(User user)
        {
            var sql = @"
                INSERT INTO users (name, last_name, date_of_birth, info, email, password_hash, role)
                VALUES (@Name, @LastName, @DateOfBirth, @Info, @Email, @PasswordHash, @Role::user_role)
                RETURNING id;
            ";

            var userId = await _connection.QuerySingleAsync<int>(sql, user.AsDapperParams());

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
                SELECT id, name, last_name, date_of_birth, info, email, password_hash, role
                FROM users;
            ";

            var users = await _connection.QueryAsync<User>(sql);
            return users;
        }

        public async Task<User?> GetById(int id)
        {
            var sql = @"
                SELECT id, name, last_name, date_of_birth, info, email, role
                FROM users
                WHERE id = @Id;
            ";

            var user = await _connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
            return user;
        }

        public async Task<bool> Update(User user)
        {
            var sql = @"
                UPDATE users
                SET name = @Name,
                    last_name = @LastName,
                    date_of_birth=@DateOfBirth,
                    info = @Info,
                    email = @Email,
                    password_hash=@PasswordHash,
                    role = @Role::user_role
                WHERE id = @Id;
            ";

            var affectedRows = await _connection.QuerySingleAsync<int>(sql, user.AsDapperParams());

            return affectedRows > 0;
        }
        public async Task<User?> ReadByEmail(string email)
        {
            const string query = "SELECT id, name, last_name, date_of_birth, info, email, password_hash, role" +
                " FROM users WHERE email = @Email";
            return await _connection.QuerySingleOrDefaultAsync<User>(query, new { Email = email });
        }
    }
}