using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public interface IMessageService
    {
        public Task<MessageDto> GetById(int Id);
        public Task<IEnumerable<MessageDto>> GetByUserId(int Id);
        public Task Create(MessageDto message);
        public Task<bool> Update(MessageDto message);
        public Task<bool> Delete(int id);
    }
}
