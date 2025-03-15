using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IMessageRepository
    {
        public Task<Message> GetById(int Id);
        public Task<IEnumerable<Message>> GetByUserId(int Id);
        public Task<int> Create(Message message);
        public Task<bool> Update(Message message);
        public Task<bool> Delete(int id);
        public Task<IEnumerable<Message>> GetAll();
    }
}
