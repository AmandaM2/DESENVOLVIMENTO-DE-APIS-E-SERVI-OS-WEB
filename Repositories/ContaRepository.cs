using Api.Interfaces;
using Api.Models;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Repositories
{
    public class ContaRepository(AppDbContext context) : IContaRepository
    {
        // Injeção de Dependência utilizando o construtor primário do C#
        private readonly AppDbContext _context = context;

        public async Task<Conta> CreateAsync(Conta conta)
        {
            await _context.Contas.AddAsync(conta);
            await _context.SaveChangesAsync();
            return conta;
        }

        public async Task<Conta?> DeleteAsync(int id)
        {
            var contaModel = await _context.Contas.FirstOrDefaultAsync(c => c.Id == id);

            if (contaModel == null)
                return null;

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
            // Melhoria: Traz os dados do Usuário anexados a cada conta listada
            return await _context.Contas.Include(c => c.Usuario).ToListAsync();
        }

        public async Task<Conta?> GetByIdAsync(int id)
        {
            // Melhoria: Usa Include em vez de FindAsync para carregar o relacionamento
            return await _context.Contas
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Conta?> UpdateAsync(int id, Conta conta)
        {
            // CORREÇÃO: Incluindo o Usuário para permitir a atualização cadastral
            var contaModel = await _context.Contas
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contaModel == null)
                return null;

            // Se o usuário da conta e as novas informações existirem, atualiza os dados dele
            if (contaModel.Usuario != null && conta.Usuario != null)
            {
                contaModel.Usuario.Nome = conta.Usuario.Nome;
                contaModel.Usuario.Email = conta.Usuario.Email;
            }

            // Atualiza os dados nativos da Conta
            contaModel.Tipo = conta.Tipo;
            contaModel.Saldo = conta.Saldo;

            await _context.SaveChangesAsync();

            return contaModel;
        }

        public async Task<Conta?> GetByTitularAsync(string titular)
        {
            return await _context.Contas
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Usuario != null && c.Usuario.Nome == titular);
        }
    }
}
