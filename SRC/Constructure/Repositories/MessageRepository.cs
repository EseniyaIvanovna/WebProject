using Domain;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly List<Message> _messages = new List<Message>();
        public Task<int> Create(Message message)
        {
            if(message == null) 
                throw new ArgumentNullException(nameof(message));
            
            _messages.Add(message);
            return Task.FromResult(message.Id) ;
        }

        public Task<bool> Delete(int id)
        {
            var message = _messages.FirstOrDefault(m => m.Id == id);

            if (message == null) 
                return Task.FromResult(false);
            
            _messages.Remove(message);
            return Task.FromResult(true);
        }

        public Task<Message> GetById(int id)
        {
            var message = _messages.FirstOrDefault(m => m.Id == id);
            return Task.FromResult(message);
        }

        public Task<IEnumerable<Message>> GetByUserId(int userId)
        {
            var messages = _messages.Where(m => m.SenderId == userId || m.ReceiverId == userId);
            return Task.FromResult(messages);
        }

        public Task<bool> Update(Message message)
        {
            if (message == null) 
                throw new ArgumentNullException(nameof(message));
            
            var existingMessage = _messages.FirstOrDefault(m => m.Id == message.Id);

            if (existingMessage == null) 
                return Task.FromResult(false);
            
            existingMessage.SenderId = message.SenderId;
            existingMessage.ReceiverId = message.ReceiverId;
            existingMessage.Text = message.Text;
            existingMessage.CreatedAt = message.CreatedAt;

            return Task.FromResult(true);
        }
    }
}
