﻿using Domain;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IPostRepository
    {
        public Task<Post?> GetById(int Id);
        public Task<int> Create(Post post);
        public Task<bool> Update(Post post);
        public Task<bool> Delete(int id);
        public Task<IEnumerable<Post>> GetAll();
        public Task DeleteByUserId(int userId);
        public Task<IEnumerable<Post>> GetByUserId(int userId);
    }
}
