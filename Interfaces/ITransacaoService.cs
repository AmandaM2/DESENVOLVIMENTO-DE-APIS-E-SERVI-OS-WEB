using Api.DTOs;
using Api.Models;
using System.Collections.Generic; // 🛠️ ADICIONADO: Necessário para usar List<>
using System.Threading.Tasks;

namespace Api.Interfaces
{
    public interface ITransacaoService
    {
        Task<decimal> ObterSaldoAsync(int contaId);
        Task RealizarDeposito(TransacaoDTO dto);
        Task RealizarSaque(TransacaoDTO dto);

        // 📜 ADICIONADO: Contrato para a rota de extrato funcionar de forma limpa
        Task<List<Transacao>> ObterExtratoAsync(int contaId);
    }
}
