using Domain;
using Microsoft.VisualBasic;
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
        public Task<int> Create(Reaction reaction)
        {
            if (reaction == null) 
                throw new ArgumentNullException(nameof(reaction));
            
            _reactions.Add(reaction);
            return Task.FromResult(reaction.Id);
        }

        public Task<bool> Delete(int id)
        {
            var reaction = _reactions.FirstOrDefault(r => r.Id == id);

            if (reaction == null) 
                return Task.FromResult(false);
           
            _reactions.Remove(reaction);
            return Task.FromResult(true);
        }

        public Task<Reaction> GetById(int Id)
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

        public Task<bool> Update(Reaction reaction)
        {
            if (reaction == null) 
                throw new ArgumentNullException(nameof(reaction));
          
            var existingReaction = _reactions.FirstOrDefault(r => r.Id == reaction.Id);

            if (existingReaction == null) 
                return Task.FromResult(false);
            
            existingReaction.Type = reaction.Type;
            existingReaction.PostId = reaction.PostId;
            existingReaction.UserId = reaction.UserId;

            return Task.FromResult(true);
        }
    }
}
