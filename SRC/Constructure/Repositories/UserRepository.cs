using Domain;
using Microsoft.VisualBasic;
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
        
        public UserRepository()
        {
            // тестовые данные 
            _users.Add(new User { Id = 1,Name="John", Age=20, Info= "john_doe", Email= "john@example.com", LastName="Doe"});
            _users.Add(new User { Id = 2,Name="Alice", Age=19, Info= "Sporty", Email= "kitty@example.com", LastName="Swan"});
            _users.Add(new User { Id = 3,Name="Bob", Age=25, Info= "artist", Email= "bob2000@example.com", LastName="Brown"});            
        }
        
        public Task<int> Create(User user)
        {
            var existingUser = _users.FirstOrDefault(m => m.Id == user.Id);
            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with the same ID already exists.");
            }
            _users.Add(user);
            return Task.FromResult(user.Id);
        }

        public Task<bool> Delete(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

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
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            existingUser.Name = user.Name;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Age = user.Age;
            existingUser.Info = user.Info;

            return Task.FromResult(true);
        }
    }
}
