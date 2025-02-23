using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Infrastructure.Repositories
{
    public interface IUserRepositori
    {
        public Task<User> ReadById(int Id);
        public Task Create(User user);
        public Task<bool> Update(User user);
    }
}
