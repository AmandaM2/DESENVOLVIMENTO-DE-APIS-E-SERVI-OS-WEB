using api.DTOs;
using api.Interfaces;
using api.Models;

namespace api.Repositories
{
    public async class TransacaoRepository(AppDbContext context) : ITransacaoRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Transacao> CreateAsync(Transacao transacao)
        {
            return await _context.Transacao.AddAsync(transacao);
        }

        public async Task<List<Transacao>> GetByContaIdAsync(int contaId)
        {
            var transacoes = await _context.Transacao.Where(t => t.ContaId == contaId).ToListAsync();
            return transacoes;
        }

        public async Task<Transacao?> RealizarDeposito(TransacaoDTO transacao)
        {
            var conta = await _context.Conta.FindAsync(transacao.ContaId);

            if (conta == null)
                return null;

            conta.Saldo += transacao.Valor;
            await _context.SaveChangesAsync();

            return new Transacao
            {
                ContaId = transacao.ContaId,
                Valor = transacao.Valor,
                Data = DateTime.Now,
                Tipo = "Depósito"
            };
        }

    }
}
