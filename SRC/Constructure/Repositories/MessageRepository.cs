using Dapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            await _connection.OpenAsync();

            var sql = @"
                INSERT INTO messages (sender_id, receiver_id, text, created_at)
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

        public async Task<bool> Delete(int id)
        {
            await _connection.OpenAsync();

            var sql = "DELETE FROM messages WHERE id = @Id";
            var affectedRows = await _connection.ExecuteAsync(sql, new { Id = id });

            return affectedRows > 0;
        }

        public async Task<Message> GetById(int id)
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM messages WHERE id = @Id";
            var message = await _connection.QuerySingleOrDefaultAsync<Message>(sql, new { Id = id });

            return message;
        }

        public async Task<IEnumerable<Message>> GetByUserId(int userId)
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM messages WHERE sender_id = @UserId OR receiver_id = @UserId";
            var messages = await _connection.QueryAsync<Message>(sql, new { UserId = userId });

            return messages;
        }

        public async Task<bool> Update(Message message)
        {
            await _connection.OpenAsync();

            var sql = @"
                UPDATE messages
                SET sender_id = @SenderId,
                    receiver_id = @ReceiverId,
                    text = @Text,
                    created_at = @CreatedAt
                WHERE id = @Id
            ";
            var affectedRows = await _connection.ExecuteAsync(sql, new
            {
                message.SenderId,
                message.ReceiverId,
                message.Text,
                message.CreatedAt,
                message.Id
            });

            return affectedRows > 0;
        }

        public async Task<IEnumerable<Message>> GetAll()
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM messages";
            var messages = await _connection.QueryAsync<Message>(sql);

            return messages;
        }
    }
}