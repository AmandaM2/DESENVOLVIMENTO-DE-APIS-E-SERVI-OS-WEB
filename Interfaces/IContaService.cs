using Api.DTOs;
using Api.Models;
using System.Threading.Tasks;

namespace Api.Interfaces
{
    public interface IContaService
    {
        Task<bool> SacarAsync(TransacaoDTO transacaoDto);

        // Regra de negócio: Registrar depósito e atualizar saldo
        Task<bool> DepositarAsync(TransacaoDTO transacaoDto);

        Task<decimal> ObterSaldoAsync(int contaId);
        Task<Conta?> BuscarPorId(int contaId);
        Task<Conta> CriarConta(ContaDTO conta);
    }
}
