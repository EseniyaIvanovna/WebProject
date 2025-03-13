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
    public class UserService : IUserService
    {
        private IUserRepository userRepository;
        private IMapper mapper;

        public UserService(IUserRepository UserRepository, IMapper Mapper)
        {
            userRepository = UserRepository;
            mapper = Mapper;
        }

        public async Task<int> Add(UserDto user)
        {
            var mappedUser = mapper.Map<User>(user);
            int userId = await userRepository.Create(mappedUser);
            return userId;
        }

        public async Task<bool> Delete(int id)
        {
            var result = await userRepository.Delete(id);
            return result;
        }

        public async Task<IEnumerable<UserDto>> GetAll()
        {
            var users = await userRepository.GetAll();
            return mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetById(int id)
        {
            var user = await userRepository.GetById(id);
            return mapper.Map<UserDto>(user);
        }
        public async Task<bool> Update(UserDto user)
        {
            var existingUser = await userRepository.GetById(user.Id);
            if (existingUser == null)
            {
                return false;
            }

            mapper.Map(user, existingUser);
            await userRepository.Update(existingUser);
            return true;
        }
    }
}
