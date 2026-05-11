using Api.Models;

namespace Api.Interfaces
{
    public interface ITokenService
    {
        string GerarToken(Conta conta);
    }
}
