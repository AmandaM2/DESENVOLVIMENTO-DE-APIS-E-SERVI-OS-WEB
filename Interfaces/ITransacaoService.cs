using Api.DTOs;
using Api.Models;


namespace Api.Interfaces
{
    public interface ITransacaoService
    {
        Task<decimal> ObterSaldoAsync(int contaId);
        Task RealizarDeposito(TransacaoDTO dto);
        Task RealizarSaque(TransacaoDTO dto);
    }
}
