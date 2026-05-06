using api.DTOs;
using api.Interfaces;
namespace api.Services
{
    public class TransacaoService : ITransacaoService
    {
        public string GetCodigo(int id)
        {
            throw new NotImplementedException();
        }

        public decimal ObterSaldo(int contaId)
        {
            throw new NotImplementedException();
        }

        public Task RealizarDeposito(TransacaoDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task RealizarSaque(TransacaoDTO dto)
        {
            throw new NotImplementedException();
        }

    }
}
