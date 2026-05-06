using api.DTOs;
using api.Models;
using System.Threading.Tasks;

namespace api.Interfaces
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
