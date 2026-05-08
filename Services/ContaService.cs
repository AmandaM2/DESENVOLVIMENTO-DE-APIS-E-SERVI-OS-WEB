using Api.DTOs;
using Api.Interfaces;
using Api.Models;
namespace Api.Services
{
    public class ContaService : IContaService
    {
        public Task<Conta?> BuscarPorId(int contaId)
        {
            throw new NotImplementedException();
        }

        public Task<Conta> CriarConta(ContaDTO conta)
        {
            throw new NotImplementedException();
        }


        public Task<bool> DepositarAsync(TransacaoDTO transacaoDto)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> ObterSaldoAsync(int contaId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SacarAsync(TransacaoDTO transacaoDto)
        {
            throw new NotImplementedException();
        }

    }
}
