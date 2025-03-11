using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Infrastructure.Repositories
{
    public interface IUserRepository
    {
        public Task<User> GetById(int Id);
        public Task<IEnumerable<User>> GetAll();
        public Task<int> Create(User user);
        public Task<bool> Update(User user);
        public Task<bool> Delete(int id);
       
    }
}
