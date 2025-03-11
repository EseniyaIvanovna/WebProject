using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public interface ICommentService
    {
        public Task<IEnumerable<CommentDto>> GetByUserId(int id);
        public Task<CommentDto> GetById(int Id);
        public Task Create(CommentDto comment);
        public Task<bool> Update(CommentDto comment);
        public Task<bool> Delete(int id);
    }
}
