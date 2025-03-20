using Domain;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.InMemoryRepositories
{
    public class InteractionInMemoryRepository : IInteractionRepository
    {
        private readonly List<Interaction> _interactions = new List<Interaction>();

        public InteractionInMemoryRepository()
        {
            // тестовые данные
            _interactions.Add(new Interaction { Id = 1, Status = Status.Friend, User1Id = 1, User2Id = 2 });
            _interactions.Add(new Interaction { Id = 2, Status = Status.Friend, User1Id = 2, User2Id = 1 });
            _interactions.Add(new Interaction { Id = 3, Status = Status.Subscriber, User1Id = 1, User2Id = 3 });
        }

        public Task<int> Create(Interaction interaction)
        {
           var existingInteraction = _interactions.FirstOrDefault(i => i.Id == interaction.Id);

            if (existingInteraction != null)
            {
                throw new InvalidOperationException("An interaction with the same ID already exists.");
            }

            _interactions.Add(interaction);
            return Task.FromResult(interaction.Id); 
        }

        public Task<bool> Delete(int id)
        {
            var interaction = _interactions.FirstOrDefault(i => i.Id == id);
            if (interaction == null)
            {
                throw new InvalidOperationException("Interaction not found.");
            }

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
            var existingInteraction = _interactions.FirstOrDefault(i => i.Id == interaction.Id);
            if (existingInteraction == null)
            {
                throw new InvalidOperationException("Interaction not found.");
            }

            existingInteraction.Status = interaction.Status;
            existingInteraction.User1Id = interaction.User1Id;
            existingInteraction.User2Id = interaction.User2Id;

            return Task.FromResult(true);
        }

        public Task<IEnumerable<Interaction>> GetAll()
        {
            return Task.FromResult(_interactions.AsEnumerable());
        }
    }
}
