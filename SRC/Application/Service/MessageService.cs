using Application.Dto;
using AutoMapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public MessageService(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<int> Create(MessageDto message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "Message cannot be null.");
            }

            var sender = await _userRepository.GetById(message.SenderId);
            if (sender == null)
            {
                throw new InvalidOperationException("Sender not found.");
            }

            var receiver = await _userRepository.GetById(message.ReceiverId);
            if (receiver == null)
            {
                throw new InvalidOperationException("Receiver not found.");
            }

            var mappedMessage = _mapper.Map<Message>(message);
            return await _messageRepository.Create(mappedMessage);
        }

        public async Task<bool> Delete(int id)
        {
            var message = await _messageRepository.GetById(id);
            if (message == null)
            {
                throw new InvalidOperationException("Message not found.");
            }

            return await _messageRepository.Delete(id);
        }

        public async Task<MessageDto> GetById(int id)
        {
            var message = await _messageRepository.GetById(id);
            return _mapper.Map<MessageDto>(message);
        }
        
        public async Task<IEnumerable<MessageDto>> GetAll()
        {
            var messages = await _messageRepository.GetAll();
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<IEnumerable<MessageDto>> GetByUserId(int userId)
        {
            var messages = await _messageRepository.GetByUserId(userId);
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> Update(MessageDto message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "Message cannot be null.");
            }

            var existingMessage = await _messageRepository.GetById(message.Id);
            if (existingMessage == null)
            {
                throw new InvalidOperationException("Message not found.");
            }

            _mapper.Map(message, existingMessage);
            return await _messageRepository.Update(existingMessage);
        }
    }
}
