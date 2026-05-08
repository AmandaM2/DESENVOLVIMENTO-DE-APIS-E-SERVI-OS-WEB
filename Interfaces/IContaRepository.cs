using Api.Models;

namespace Api.Interfaces
{
    public interface IContaRepository
    {
        // Métodos que nosso repositório de Contas será OBRIGADO a ter
        Task<Conta> CreateAsync(Conta conta);
        Task<Conta?> DeleteAsync(int id);
        Task<bool> ExisteContaAsync(int id);
        Task<List<Conta>> GetAllAsync();
        Task<Conta?> GetByIdAsync(int id);
        Task<Conta?> UpdateAsync(int id, Conta conta);
        Task<Conta?> GetByTitularAsync(string titular);
    }
}
