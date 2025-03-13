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
    public class CommentService : ICommentService
    {
        private ICommentRepository commentRepository;
        private IMapper mapper;

        public CommentService(ICommentRepository CommentRepository, IMapper Mapper)
        {
            commentRepository = CommentRepository;
            mapper = Mapper;
        }
        public async Task<int> Create(CommentDto comment)
        {
            var mappedComment = mapper.Map<Comment>(comment);
            int commentId = await commentRepository.Create(mappedComment);
            return commentId;
        }

        public async Task<bool> Delete(int id)
        {
            var result = await commentRepository.Delete(id);
            return result;
        }

        public async Task<CommentDto> GetById(int id)
        {
            var comment = await commentRepository.GetById(id);
            return mapper.Map<CommentDto>(comment);
        }

        public async Task<IEnumerable<CommentDto>> GetByUserId(int userId)
        {
            var comments = await commentRepository.GetByUserId(userId);
            return mapper.Map<IEnumerable<CommentDto>>(comments);
        }

        public async Task<bool> Update(CommentDto comment)
        {
            var existingComment = await commentRepository.GetById(comment.Id);
            if (existingComment == null)
            {
                return false;
            }

            mapper.Map(comment, existingComment);
            await commentRepository.Update(existingComment);
            return true;
        }
        public async Task<IEnumerable<CommentDto>> GetAll()
        {
            var comments = await commentRepository.GetAll();
            return mapper.Map<IEnumerable<CommentDto>>(comments);
        }

    }
}
