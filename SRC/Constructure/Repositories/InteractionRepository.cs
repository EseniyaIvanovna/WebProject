using Domain;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class InteractionRepository : IInteractionRepository
    {
        private readonly List<Interaction> _interactions = new List<Interaction>();
        public Task<int> Create(Interaction interaction)
        {
            if (interaction == null)
                throw new ArgumentNullException(nameof(interaction));
            
            _interactions.Add(interaction);
            return Task.FromResult(interaction.Id); 
        }

        public Task<bool> Delete(int id)
        {
            var interaction = _interactions.FirstOrDefault(i => i.Id == id);

            if (interaction == null) 
                return Task.FromResult(false);
            
            _interactions.Remove(interaction);
            return Task.FromResult(true);
        }

        public Task<Interaction> GetById(int id)
        {
            var interaction = _interactions.FirstOrDefault(i => i.Id == id);
            return Task.FromResult(interaction);
        }

        public Task<IEnumerable<Interaction>> GetByStatus(Status status)
        {
            var interactions = _interactions.Where(i => i.Status == status);
            return Task.FromResult(interactions);
        }

        public Task<bool> Update(Interaction interaction)
        {
            if (interaction == null) 
                throw new ArgumentNullException(nameof(interaction));
           
            var existingInteraction = _interactions.FirstOrDefault(i => i.Id == interaction.Id);

            if (existingInteraction == null) 
                return Task.FromResult(false);
            
           
            existingInteraction.Status = interaction.Status;
            existingInteraction.User1Id = interaction.User1Id;
            existingInteraction.User2Id = interaction.User2Id;
           
            return Task.FromResult(true);
        }
    }
}
