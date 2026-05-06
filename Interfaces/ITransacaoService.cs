using api.DTOs;

namespace api.Interfaces
{
    public interface ITransacaoService
    {

        string GetCodigo(int id);
        decimal ObterSaldo(int contaId);
        Task RealizarDeposito(TransacaoDTO dto);
        Task RealizarSaque(TransacaoDTO dto);
    }
}
