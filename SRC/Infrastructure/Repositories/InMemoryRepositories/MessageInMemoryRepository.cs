using Domain;
using Infrastructure.Repositories.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Repositories.InMemoryRepositories
{
    [ExcludeFromCodeCoverage]
    public class MessageInMemoryRepository : IMessageRepository
    {
        private readonly List<Message> _messages = new List<Message>();

        public MessageInMemoryRepository()
        {
            // тестовые данные
            _messages.Add(new Message { Id = 1, Text = "Hello!", SenderId = 1, ReceiverId = 2 });
            _messages.Add(new Message { Id = 2, Text = "How are you?", SenderId = 2, ReceiverId = 1 });
            _messages.Add(new Message { Id = 3, Text = "I'm fine, thanks!", SenderId = 1, ReceiverId = 2 });
        }

        public Task<int> Create(Message message)
        {
            var existingMessage = _messages.FirstOrDefault(m => m.Id == message.Id);
            if (existingMessage != null)
            {
                throw new InvalidOperationException("A message with the same ID already exists.");
            }
            _messages.Add(message);
            return Task.FromResult(message.Id) ;
        }

        public Task<bool> Delete(int id)
        {
            var message = _messages.FirstOrDefault(m => m.Id == id);
            if (message == null)
            {
                throw new InvalidOperationException("Message not found.");
            }

            _messages.Remove(message);
            return Task.FromResult(true);
        }

#pragma warning disable CS8613 // Допустимость значения NULL для ссылочных типов в возвращаемом типе не совпадает с явно реализованным членом.
        public Task<Message> GetById(int id)
#pragma warning restore CS8613 // Допустимость значения NULL для ссылочных типов в возвращаемом типе не совпадает с явно реализованным членом.
        {
            var message = _messages.First(m => m.Id == id);
            return Task.FromResult(message);
        }

        public Task<IEnumerable<Message>> GetByUserId(int userId)
        {
            var messages = _messages.Where(m => m.SenderId == userId || m.ReceiverId == userId);
            return Task.FromResult(messages);
        }

        public Task<bool> Update(Message message)
        {
            var existingMessage = _messages.FirstOrDefault(m => m.Id == message.Id);
            if (existingMessage == null)
            {
                throw new InvalidOperationException("Message not found.");
            }

            existingMessage.SenderId = message.SenderId;
            existingMessage.ReceiverId = message.ReceiverId;
            existingMessage.Text = message.Text;
            existingMessage.CreatedAt = message.CreatedAt;

            return Task.FromResult(true);
        }

        public Task<IEnumerable<Message>> GetAll()
        {
            return Task.FromResult(_messages.AsEnumerable());
        }

        Task IMessageRepository.DeleteMessagesByUser(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
