using Api.Models;

namespace Api.Interfaces;

public interface ITokenService
{
    // Altere de 'Conta conta' para 'Usuario usuario'
    string GerarToken(Usuario usuario);
}
