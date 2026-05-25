using System.Collections.Generic;

namespace Api.Models
{
    public class Conta
    {
        // Construtor vazio obrigatório para o Entity Framework e inicializadores
        public Conta()
        {
        }

        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal Saldo { get; set; }

        // Vinculando a Conta ao Usuário (Chave Estrangeira explícita)
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public decimal LimiteCartao { get; set; }
        public decimal Cofrinho { get; set; }

        // Relacionamento: Uma conta pode ter muitas transações
        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }
}
