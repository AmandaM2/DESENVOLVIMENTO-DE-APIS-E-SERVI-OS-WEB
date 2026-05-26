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
            // Traz os dados do Usuário e Transações anexados
            return await _context.Contas
                .Include(c => c.Usuario)
                .Include(c => c.Transacoes)
                .ToListAsync();
        }

        public async Task<Conta?> GetByIdAsync(int id)
        {
            return await _context.Contas
                .Include(c => c.Usuario)
                .Include(c => c.Transacoes) // Adicionado para consistência
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Conta?> UpdateAsync(int id, Conta conta)
        {
            var contaModel = await _context.Contas
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contaModel == null)
                return null;

            if (contaModel.Usuario != null && conta.Usuario != null)
            {
                contaModel.Usuario.Nome = conta.Usuario.Nome;
                contaModel.Usuario.Email = conta.Usuario.Email;
            }

            // --- CORREÇÃO MAPEAMENTO: Salva todas as novas propriedades no MySQL ---
            contaModel.Tipo = conta.Tipo;
            contaModel.Saldo = conta.Saldo;
            contaModel.Cofrinho = conta.Cofrinho;       // <-- ADICIONADO PARA O COFRINHO
            contaModel.LimiteCartao = conta.LimiteCartao; // <-- ADICIONADO PARA O LIMITE

            await _context.SaveChangesAsync();

            return contaModel;
        }

        public async Task<Conta?> GetByTitularAsync(string titular)
        {
            return await _context.Contas
                .Include(c => c.Usuario)
                .Include(c => c.Transacoes) // <-- CORREÇÃO: Alimenta o histórico e o gráfico do script.js
                .FirstOrDefaultAsync(c => c.Usuario != null && c.Usuario.Nome == titular);
        }

        // ==========================================================================
        // 🔥 NOVO: ADICIONADO PARA SALVAR O HISTÓRICO DE TRANSAÇÕES DO COFRINHO
        // ==========================================================================
        public async Task AdicionarTransacaoAsync(Transacao transacao)
        {
            await _context.Transacoes.AddAsync(transacao);
            await _context.SaveChangesAsync();
        }
    }
}