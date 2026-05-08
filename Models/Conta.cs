using System.Collections.Generic;
namespace Api.Models;

public class Conta
{
    public int Id { get; set; }
    public string Titular { get; set; } = string.Empty;

    public string Senha { get; set; } = string.Empty;

    // Tipos: "Corrente", "Poupança" ou "Empresarial"
    public string Tipo { get; set; } = string.Empty;

    public decimal Saldo { get; set; }

    // Relacionamento: Uma conta pode ter muitas transações
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}
