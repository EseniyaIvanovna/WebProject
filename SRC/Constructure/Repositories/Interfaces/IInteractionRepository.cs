using Domain;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IInteractionRepository
    {
        public Task<Interaction> GetById(int Id);
        public Task<IEnumerable<Interaction>> GetByStatus(Status status);
        public Task<int> Create(Interaction interaction);
        public Task<bool> Update(Interaction interaction);
        public Task<bool> Delete(int id);
        public Task<IEnumerable<Interaction>> GetAll();
    }
}
