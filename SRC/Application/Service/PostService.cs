using Application.Dto;
using AutoMapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IReactionRepository _reactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public PostService(IPostRepository postRepository, ICommentRepository commentRepository, IReactionRepository reactionRepository, IUserRepository userRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _reactionRepository = reactionRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<int> Create(PostDto post)
        {
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post), "Post cannot be null.");
            }

            var user = await _userRepository.GetById(post.UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var mappedPost = _mapper.Map<Post>(post);
            return await _postRepository.Create(mappedPost);
        }

        public async Task<bool> Delete(int id)
        {
            var post = await _postRepository.GetById(id);
            if (post == null)
            {
                throw new InvalidOperationException("Post not found.");
            }

            await _commentRepository.DeleteByPostId(id);

            await _reactionRepository.DeleteByPostId(id);

            return await _postRepository.Delete(id);
        }

        public async Task<IEnumerable<PostDto>> GetAll()
        {
            var posts = await _postRepository.GetAll();
            return _mapper.Map<IEnumerable<PostDto>>(posts);
        }

        public async Task<PostDto> GetById(int id)
        {
            var post = await _postRepository.GetById(id);
            return _mapper.Map<PostDto>(post);
        }

        public async Task<bool> Update(PostDto post)
        {
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post), "Post cannot be null.");
            }

            var existingPost = await _postRepository.GetById(post.Id);
            if (existingPost == null)
            {
                throw new InvalidOperationException("Post not found.");
            }

            _mapper.Map(post, existingPost);
            return await _postRepository.Update(existingPost);
        }
    }
}
