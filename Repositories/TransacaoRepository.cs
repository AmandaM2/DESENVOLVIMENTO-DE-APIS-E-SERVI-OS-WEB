using api.DTOs;
using api.Interfaces;
using api.Models;

namespace api.Repositories
{
    public class TransacaoRepository : ITransacaoRepository
    {
        public Task<Transacao> AddAsync(Transacao transacao)
        {
            throw new NotImplementedException();
        }

        public Task<List<Transacao>> GetByContaIdAsync(int contaId)
        {
            throw new NotImplementedException();
        }

        public Task<Transacao?> RealizarDeposito(TransacaoDTO transacao)
        {
            throw new NotImplementedException();
        }

    }
}
