using api.Models;

namespace api.Interfaces;

public interface IContaRepository
{
    // Lista todas as contas (Útil para o GET do Controller)
    Task<List<Conta>> GetAllAsync();

    // Busca uma conta específica pelo ID (Importante para verificar o saldo)
    Task<Conta?> GetByIdAsync(int id);

    // Cria uma nova conta (CRUD completo exigido)
    Task<Conta> CreateAsync(Conta conta);

    // Atualiza os dados da conta (Como o saldo após um saque ou depósito)
    Task<Conta?> UpdateAsync(int id, Conta conta);

    // Remove uma conta
    Task<Conta?> DeleteAsync(int id);

    // Verifica se uma conta existe antes de tentar realizar uma transação
    Task<bool> ExisteContaAsync(int id);
}
