using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users = new List<User>(); 
        public Task Create(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task<bool> Delete(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);

            if (user == null) return Task.FromResult(false);
            
            _users.Remove(user);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<User>> GetAll()
        {
            return Task.FromResult(_users.AsEnumerable());
        }

        public Task<User> GetById(int Id)
        {
            var user = _users.FirstOrDefault(u => u.Id == Id);
            return Task.FromResult(user);
        }

        public Task<bool> Update(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
           
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);

            if (existingUser == null) return Task.FromResult(false);
            
            existingUser.Name = user.Name;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Age = user.Age;
            existingUser.Info = user.Info;
            
            return Task.FromResult(true); 
        }
    }
}
