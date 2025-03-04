﻿using Domain;
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
        public Task Create(Message message)
        {
            if(message == null) throw new ArgumentNullException(nameof(message));
            
            _messages.Add(message);
            return Task.CompletedTask;
        }

        public Task<bool> Delete(int id)
        {
            var message = _messages.FirstOrDefault(m => m.Id == id);

            if (message == null) return Task.FromResult(false);
            
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
            var messages = _messages.Where(m => m.Sender.Id == userId || m.Reciever.Id == userId).AsEnumerable();
            return Task.FromResult(messages);
        }

        public Task<bool> Update(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            
            var existingMessage = _messages.FirstOrDefault(m => m.Id == message.Id);

            if (existingMessage == null) return Task.FromResult(false);
            
            existingMessage.Sender = message.Sender;
            existingMessage.Reciever = message.Reciever;
            existingMessage.Text = message.Text;
            existingMessage.DateTime = message.DateTime;

            return Task.FromResult(true);
        }
    }
}
