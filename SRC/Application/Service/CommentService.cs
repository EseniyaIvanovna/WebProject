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
    public class CommentService : ICommentService
    {
        private ICommentRepository commentRepository;
        private IMapper mapper;

        public CommentService(ICommentRepository commentRepository, IMapper mapper)
        {
            this.commentRepository = commentRepository;
            this.mapper = mapper;
        }
        public Task Create(CommentDto comment)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CommentDto> GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CommentDto>> GetByUserId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(CommentDto comment)
        {
            throw new NotImplementedException();
        }
    }
}
