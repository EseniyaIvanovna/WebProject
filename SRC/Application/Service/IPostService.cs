using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public interface IPostService
    {
        public Task<PostDto> GetById(int Id);
        public Task<IEnumerable<PostDto>> GetAll();
        public Task<int> Create(PostDto post);
        public Task<bool> Update(PostDto post);
        public Task<bool> Delete(int id);
    }
}
