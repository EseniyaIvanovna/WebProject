using Dapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Npgsql;

namespace Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly NpgsqlConnection _connection;

        public MessageRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> Create(Message message)
        {
            message.CreatedAt = DateTime.UtcNow;

            var sql = @"
                INSERT INTO messages (senderId, receiverId, text, createdAt)
                VALUES (@SenderId, @ReceiverId, @Text, @CreatedAt)
                RETURNING id
            ";
            var messageId = await _connection.QuerySingleAsync<int>(sql, new
            {
                message.SenderId,
                message.ReceiverId,
                message.Text,
                message.CreatedAt
            });

            return messageId;
        }

        public async Task Delete(int id)
        {
            var sql = "DELETE FROM messages WHERE id = @Id";
            await _connection.ExecuteAsync(sql, new { Id = id });

        }

        public async Task DeleteMessagesByUser(int userId, NpgsqlTransaction transaction)
        {
            await _connection.ExecuteAsync(
                "DELETE FROM messages WHERE senderId = @UserId OR receiverId = @UserId",
                new { UserId = userId },
                transaction: transaction);
        }

        public async Task<Message?> GetById(int id)
        {
            var sql = @"
                SELECT id, senderId, receiverId, text, createdAt
                FROM messages
                WHERE id = @Id;
            ";
            var message = await _connection.QuerySingleOrDefaultAsync<Message>(sql, new { Id = id });

            return message;
        }

        public async Task<IEnumerable<Message>> GetByUserId(int userId)
        {
            var sql = "SELECT id, senderId, receiverId, text, createdAt FROM messages WHERE senderId = @UserId OR receiverId = @UserId";
            var messages = await _connection.QueryAsync<Message>(sql, new { UserId = userId });

            return messages;
        }

        public async Task Update(Message message)
        {
            var sql = @"
                UPDATE messages
                SET text = @Text
                WHERE id = @Id
            ";

            await _connection.ExecuteAsync(sql, new
            {
                message.Text,
                message.Id
            });

        }

        public async Task<IEnumerable<Message>> GetAll()
        {
            var sql = @"
                SELECT id, senderId, receiverId, text, createdAt
                FROM messages;
            ";

            var messages = await _connection.QueryAsync<Message>(sql);

            return messages;
        }
    }
}