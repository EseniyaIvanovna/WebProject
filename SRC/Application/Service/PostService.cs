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
    public class PostService : IPostService
    {
        private IPostRepository postRepository;
        private IMapper mapper;

        public PostService(IPostRepository PostRepository, IMapper Mapper)
        {
            postRepository = PostRepository;
            mapper = Mapper;
        }

        public async Task<int> Create(PostDto post)
        {
            var mappedPost = mapper.Map<Post>(post);
            int postId = await postRepository.Create(mappedPost);
            return postId;
        }

        public async Task<bool> Delete(int id)
        {
            var result = await postRepository.Delete(id);
            return result;
        }

        public async Task<IEnumerable<PostDto>> GetAll()
        {
            var posts = await postRepository.GetAll();
            return mapper.Map<IEnumerable<PostDto>>(posts);
        }

        public async Task<PostDto> GetById(int id)
        {
            var post = await postRepository.GetById(id);
            return mapper.Map<PostDto>(post);
        }

        public async Task<bool> Update(PostDto post)
        {
            var existingPost = await postRepository.GetById(post.Id);
            if (existingPost == null)
            {
                return false;
            }

            mapper.Map(post, existingPost);
            await postRepository.Update(existingPost);
            return true;
        }
    }
}
