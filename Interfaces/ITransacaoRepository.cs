using System.Collections.Generic;
using System.Threading.Tasks;
using Api.DTOs;
using Api.Models;
namespace Api.Interfaces
{
    public interface ITransacaoRepository
    {
        Task<Transacao> CreateAsync(Transacao transacao);

        // Retorna o extrato de uma conta específica
        Task<List<Transacao>> GetByContaIdAsync(int contaId);
        Task<Transacao?> RealizarDeposito(TransacaoDTO transacao);
    }
}
