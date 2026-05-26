using Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Interfaces
{
    public interface IContaRepository
    {
        // Métodos obrigatórios do repositório
        Task<Conta> CreateAsync(Conta conta);
        Task<Conta?> DeleteAsync(int id);
        Task<bool> ExisteContaAsync(int id);
        Task<List<Conta>> GetAllAsync();
        Task<Conta?> GetByIdAsync(int id);
        Task<Conta?> UpdateAsync(int id, Conta conta);
        Task<Conta?> GetByTitularAsync(string titular);

        // 🔥 CERTIFIQUE-SE DE QUE ESTA LINHA ESTÁ AQUI:
        Task AdicionarTransacaoAsync(Transacao transacao);
    }
}