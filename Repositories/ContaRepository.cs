using api.Interfaces;
using api.Models;

namespace api.Repositories
{
    public class ContaRepository : IContaRepository
    {
        public Task<Conta> CreateAsync(Conta conta)
        {
            throw new NotImplementedException();
        }

        public Task<Conta?> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExisteContaAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Conta>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Conta?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Conta?> UpdateAsync(int id, Conta conta)
        {
            throw new NotImplementedException();
        }
    }
}
