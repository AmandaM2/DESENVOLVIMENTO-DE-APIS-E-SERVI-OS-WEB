using Api.Interfaces;
using Api.Models;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class ContaRepository(AppDbContext context) : IContaRepository
    {

        // Injeção de Dependência
        private readonly AppDbContext _context = context;

        public async Task<Conta> CreateAsync(Conta conta)
        {
            await _context.Contas.AddAsync(conta);
            await _context.SaveChangesAsync(); // CORREÇÃO: Necessário para salvar de fato!
            return conta;
        }

        public async Task<Conta?> DeleteAsync(int id)
        {
            var contaModel = await _context.Contas.FirstOrDefaultAsync(c => c.Id == id);

            if (contaModel == null)
                return contaModel;

            _context.Contas.Remove(contaModel);
            await _context.SaveChangesAsync();

            return contaModel;
        }

        public async Task<bool> ExisteContaAsync(int id)
        {
            return await _context.Contas.AnyAsync(c => c.Id == id);
        }

        public async Task<List<Conta>> GetAllAsync()
        {
            return await _context.Contas.ToListAsync();
        }

        public async Task<Conta?> GetByIdAsync(int id)
        {
            var contaModel = await _context.Contas.FindAsync(id);

            if (contaModel == null)
                return contaModel;

            return contaModel;
        }

        public async Task<Conta?> UpdateAsync(int id, Conta conta)
        {
            var contaModel = await _context.Contas.FirstOrDefaultAsync(c => c.Id == id);

            if (contaModel == null)
                return contaModel;

            contaModel.Titular = conta.Titular;
            contaModel.Tipo = conta.Tipo;
            contaModel.Saldo = conta.Saldo;
            await _context.SaveChangesAsync();

            return contaModel;
        }

        public async Task<Conta?> GetByTitularAsync(string titular)
        {
            // Busca a primeira conta que tiver o mesmo nome de titular
            return await _context.Contas.FirstOrDefaultAsync(c => c.Titular == titular);
        }
    }
}
