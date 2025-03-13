using Application.Dto;
using AutoMapper;
using Domain;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class MessageService : IMessageService
    {
        private IMessageRepository messRepository;
        private IMapper mapper;

        public MessageService(IMessageRepository MessRepository, IMapper Mapper)
        {
            messRepository = MessRepository;
            mapper = Mapper;
        }

        public async Task<int> Create(MessageDto message)
        {
            var mappedMessage = mapper.Map<Message>(message);
            int messageId = await messRepository.Create(mappedMessage);
            return messageId;
        }

        public async Task<bool> Delete(int id)
        {
            var result = await messRepository.Delete(id);
            return result;
        }

        public async Task<MessageDto> GetById(int id)
        {
            var message = await messRepository.GetById(id);
            return mapper.Map<MessageDto>(message);
        }
        public async Task<IEnumerable<MessageDto>> GetAll()
        {
            var messages = await messRepository.GetAll();
            return mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<IEnumerable<MessageDto>> GetByUserId(int userId)
        {
            var messages = await messRepository.GetByUserId(userId);
            return mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> Update(MessageDto message)
        {
            var existingMessage = await messRepository.GetById(message.Id);
            if (existingMessage == null)
            {
                return false;
            }

            mapper.Map(message, existingMessage);
            await messRepository.Update(existingMessage);
            return true;
        }
    }
}
