using Domain;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IInteractionRepository
    {
        public Task<Interaction> GetById(int Id);
        public Task<IEnumerable<Interaction>> GetByStatus(Status status);
        public Task Create(Interaction interaction);
        public Task<bool> Update(Interaction interaction);
        public Task<bool> Delete(int id);
    }
}
