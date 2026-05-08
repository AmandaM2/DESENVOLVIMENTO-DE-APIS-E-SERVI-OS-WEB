using Api.DTOs;
using Api.Interfaces;
using Api.Models;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class TransacaoRepository(AppDbContext context) : ITransacaoRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Transacao> CreateAsync(Transacao transacao)
        {
            await _context.Transacoes.AddAsync(transacao); // CORREÇÃO: Transacoes no plural
            await _context.SaveChangesAsync(); // CORREÇÃO: Necessário para salvar no banco
            return transacao;
        }

        public async Task<List<Transacao>> GetByContaIdAsync(int contaId)
        {
            return await _context.Transacoes.Where(t => t.ContaId == contaId).ToListAsync();
        }

        public async Task<Transacao?> RealizarDeposito(TransacaoDTO transacao)
        {
            var conta = await _context.Contas.FindAsync(transacao.ContaId);

            if (conta == null)
                return null;

            conta.Saldo += transacao.Valor;
            await _context.SaveChangesAsync();

            var novaTransacao = new Transacao
            {
                ContaId = transacao.ContaId,
                Valor = transacao.Valor,
                DataHora = DateTime.Now, // Usei DataHora, que é o padrão do model anterior
                                         // Tipo = "Depósito" // O jeito de preencher o tipo depende se você está usando Enum ou string no seu Model final
            };

            // CORREÇÃO: Precisamos salvar essa nova transação no banco também!
            await _context.Transacoes.AddAsync(novaTransacao);
            await _context.SaveChangesAsync();

            return novaTransacao;
        }
    }
}
