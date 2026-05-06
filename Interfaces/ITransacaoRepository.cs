using System.Collections.Generic;
using System.Threading.Tasks;
using api.DTOs;
using api.Models;
namespace api.Interfaces
{
    public interface ITransacaoRepository
    {
        Task<Transacao> AddAsync(Transacao transacao);

        // Retorna o extrato de uma conta específica
        Task<List<Transacao>> GetByContaIdAsync(int contaId);
        Task<Transacao?> RealizarDeposito(TransacaoDTO transacao);
    }
}
