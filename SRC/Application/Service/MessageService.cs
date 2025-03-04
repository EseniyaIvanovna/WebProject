using Application.Dto;
using AutoMapper;
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

        public MessageService(IMessageRepository messRepository, IMapper mapper)
        {
            this.messRepository = messRepository;
            this.mapper = mapper;
        }
        Task IMessageService.Create(MessageDto message)
        {
            throw new NotImplementedException();
        }

        Task<bool> IMessageService.Delete(int id)
        {
            throw new NotImplementedException();
        }

        Task<MessageDto> IMessageService.GetById(int Id)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<MessageDto>> IMessageService.GetByUserId(int Id)
        {
            throw new NotImplementedException();
        }

        Task<bool> IMessageService.Update(MessageDto message)
        {
            throw new NotImplementedException();
        }
    }
}
