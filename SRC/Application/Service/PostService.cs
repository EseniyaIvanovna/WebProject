using Application.Dto;
using AutoMapper;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class PostService : IPostService
    {
        private IPostRepository postRepository;
        private IMapper mapper;

        public PostService(IPostRepository postRepository, IMapper mapper)
        {
            this.postRepository = postRepository;
            this.mapper = mapper;
        }
        public Task Create(PostDto post)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PostDto>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<PostDto> GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(PostDto post)
        {
            throw new NotImplementedException();
        }
    }
}
