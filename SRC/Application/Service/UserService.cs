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

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task Add(UserDto user)
        {
            var mappedUser = mapper.Map<User>(user);
            await userRepository.Create(mappedUser);
        }

        public async Task<bool> Delete(int id)
        {
            var result = await userRepository.Delete(id);
            return result;
        }

        Task<IEnumerable<UserDto>> IUserService.GetAll()
        {
            throw new NotImplementedException();
        }

        Task<UserDto> IUserService.GetById(int Id)
        {
            throw new NotImplementedException();
        }

        Task<bool> IUserService.Update(UserDto user)
        {
            throw new NotImplementedException();
        }
    }
}
