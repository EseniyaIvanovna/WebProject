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
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public CommentService(ICommentRepository commentRepository, IUserRepository userRepository, IPostRepository postRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _postRepository = postRepository;
            _mapper = mapper;
        }
       
        public async Task<int> Create(CommentDto comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            var user = await _userRepository.GetById(comment.UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var post = await _postRepository.GetById(comment.PostId);
            if (post == null)
            {
                throw new InvalidOperationException("Post not found.");
            }

            var mappedComment = _mapper.Map<Comment>(comment);
            return await _commentRepository.Create(mappedComment);
        }
        
        public async Task<bool> Update(CommentDto comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment), "Comment cannot be null.");
            }

            var existingComment = await _commentRepository.GetById(comment.Id);
            if (existingComment == null)
            {
                throw new InvalidOperationException("Comment not found.");
            }

            _mapper.Map(comment, existingComment);
            return await _commentRepository.Update(existingComment);
        }

        public async Task<bool> Delete(int id)
        {
            var comment = await _commentRepository.GetById(id);
            if (comment == null)
            {
                throw new InvalidOperationException("Comment not found.");
            }

            return await _commentRepository.Delete(id);
        }

        public async Task<CommentDto> GetById(int id)
        {
            var comment = await _commentRepository.GetById(id);
            return _mapper.Map<CommentDto>(comment);
        }

        public async Task<IEnumerable<CommentDto>> GetByUserId(int userId)
        {
            var comments = await _commentRepository.GetByUserId(userId);
            return _mapper.Map<IEnumerable<CommentDto>>(comments);
        }
   
        public async Task<IEnumerable<CommentDto>> GetAll()
        {
            var comments = await _commentRepository.GetAll();
            return _mapper.Map<IEnumerable<CommentDto>>(comments);
        }
    }
}
