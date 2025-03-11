using Application.Dto;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public interface IUserService
    {
        public Task Add(UserDto user);
        public Task<bool> Delete(int id);
        public Task<UserDto> GetById(int Id);
        public Task<IEnumerable<UserDto>> GetAll();
        public Task<bool> Update(UserDto user);
        

    }
}
