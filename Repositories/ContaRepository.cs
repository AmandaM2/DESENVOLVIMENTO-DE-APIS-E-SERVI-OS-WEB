using api.Interfaces;
using api.Models;
using api.Data;

namespace api.Repositories
{
    public class ContaRepository(AppDbContext context) : IContaRepository
    {

        // Injeção de Dependência
        private readonly AppDbContext _context = context;

        public async Task<Conta> CreateAsync(Conta conta)
        {
            return await _context.Conta.AddAsync(conta);
        }

        public async Task<Conta?> DeleteAsync(int id)
        {
            var contaModel = await _context.Conta.FirstOrDefault(c => c.Id == id);

            if (contaModel == null)
                return contaModel;

            _context.Conta.Remove(contaModel);
            await _context.SaveChangeAsync();

            return contaModel;
        }

        public async Task<bool> ExisteContaAsync(int id)
        {
            return await _context.Conta.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Conta>> GetAllAsync()
        {
            return await _context.Conta.ToListAsync();
        }

        public async Task<Conta?> GetByIdAsync(int id)
        {
            var contaModel = await _context.Conta.Find(id);

            if (contaModel == null)
                return contaModel;

            return contaModel;
        }

        public async Task<Conta?> UpdateAsync(int id, Conta conta)
        {
            var contaModel = await _context.Conta.FirstOrDefault(c => c.Id == id);

            if (contaModel == null)
                return contaModel;

            contaModel.Titular = conta.Titular;
            contaModel.Tipo = conta.Tipo;
            contaModel.Saldo = conta.Saldo;
            await _context.SaveChangeAsync();

            return contaModel;
        }
    }
}
