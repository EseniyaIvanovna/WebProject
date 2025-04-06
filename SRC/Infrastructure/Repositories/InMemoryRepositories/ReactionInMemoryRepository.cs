using Domain;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Npgsql;

namespace Infrastructure.Repositories.InMemoryRepositories
{
    public class ReactionInMemoryRepository : IReactionRepository
    {
        private readonly List<Reaction> _reactions = new List<Reaction>();
        
        public ReactionInMemoryRepository()
        {
            // тестовые данные
            _reactions.Add(new Reaction { Id = 1, Type = ReactionType.Like, UserId = 1, PostId = 1 });
            _reactions.Add(new Reaction { Id = 2, Type = ReactionType.Heart, UserId = 2, PostId = 1 });
            _reactions.Add(new Reaction { Id = 3, Type = ReactionType.Dislike, UserId = 1, PostId = 2 });
        }
        
        public Task<int> Create(Reaction reaction)
        {
            var existingReaction = _reactions.FirstOrDefault(m => m.Id == reaction.Id);
            if (existingReaction != null)
            {
                throw new InvalidOperationException("A reaction with the same ID already exists.");
            }
            _reactions.Add(reaction);
            return Task.FromResult(reaction.Id);
        }

        public Task Delete(int id)
        {
            var reaction = _reactions.FirstOrDefault(r => r.Id == id);
            if (reaction == null)
            {
                throw new InvalidOperationException("Reaction not found.");
            }

            _reactions.Remove(reaction);
            return Task.FromResult(true);
        }

        public Task<Reaction?> GetById(int Id)
        {
            var reaction = _reactions.FirstOrDefault(r => r.Id == Id);
            return Task.FromResult(reaction);
        }

        public Task<IEnumerable<Reaction>> GetByPostId(int postId)
        {
            var reactions = _reactions.Where(r => r.PostId == postId);
            return Task.FromResult(reactions);
        }

        public Task<IEnumerable<Reaction>> GetByUserId(int Id)
        {
            var reactions = _reactions.Where(r => r.UserId == Id);
            return Task.FromResult(reactions);
        }

        public Task Update(Reaction reaction)
        {
            var existingReaction = _reactions.FirstOrDefault(r => r.Id == reaction.Id);
            if (existingReaction == null)
            {
                throw new InvalidOperationException("Reaction not found.");
            }

            existingReaction.Type = reaction.Type;
            existingReaction.PostId = reaction.PostId;
            existingReaction.UserId = reaction.UserId;

            return Task.FromResult(true);
        }

        public Task<IEnumerable<Reaction>> GetAll()
        {
            return Task.FromResult(_reactions.AsEnumerable());
        }

        public Task DeleteByPostId(int postId)
        {
            var reactionsToDelete = _reactions.Where(r => r.PostId == postId).ToList();
            foreach (var reaction in reactionsToDelete)
            {
                _reactions.Remove(reaction);
            }
            return Task.CompletedTask;
        }

        public Task DeleteByUserId(int userId)
        {
            var reactionsToDelete = _reactions.Where(r => r.UserId == userId).ToList();
            foreach (var reaction in reactionsToDelete)
            {
                _reactions.Remove(reaction);
            }
            return Task.CompletedTask;
        }

        Task<bool> IReactionRepository.Exists(int userId, int postId)
        {
            throw new NotImplementedException();
        }

        //Task IReactionRepository.DeleteByPostOwnerId(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        Task IReactionRepository.DeleteByUserId(int userId, NpgsqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        Task IReactionRepository.DeleteByPostOwnerId(int userId, NpgsqlTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
