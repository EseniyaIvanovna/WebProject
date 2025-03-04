using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ReactionRepository : IReactionRepository
    {
        private readonly List<Reaction> _reactions = new List<Reaction>();
        public Task Create(Reaction reaction)
        {
            if (reaction == null) throw new ArgumentNullException(nameof(reaction));
            
            _reactions.Add(reaction);
            return Task.CompletedTask;
        }

        public Task<bool> Delete(int id)
        {
            var reaction = _reactions.FirstOrDefault(r => r.Id == id);

            if (reaction == null) return Task.FromResult(false);
           
            _reactions.Remove(reaction);
            return Task.FromResult(true);
        }

        public Task<Reaction> GetById(int Id)
        {
            var reaction = _reactions.FirstOrDefault(r => r.Id == Id);
            return Task.FromResult(reaction);
        }

        public Task<IEnumerable<Reaction>> GetByPosId(int postId)
        {
            var reactions = _reactions.Where(r => r.Post.Id == postId).AsEnumerable();
            return Task.FromResult(reactions);
        }

        public Task<IEnumerable<Reaction>> GetByUserId(int Id)
        {
            var reactions = _reactions.Where(r => r.User.Id == Id).AsEnumerable();
            return Task.FromResult(reactions);
        }

        public Task<bool> Update(Reaction reaction)
        {
            if (reaction == null) throw new ArgumentNullException(nameof(reaction));
          
            var existingReaction = _reactions.FirstOrDefault(r => r.Id == reaction.Id);

            if (existingReaction == null) return Task.FromResult(false);
            
            existingReaction.Type = reaction.Type;
            existingReaction.Post = reaction.Post;
            existingReaction.User = reaction.User;

            return Task.FromResult(true);
        }
    }
}
