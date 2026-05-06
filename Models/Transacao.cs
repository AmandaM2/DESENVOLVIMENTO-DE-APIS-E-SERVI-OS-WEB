using System;

namespace api.Models
{
    public class Transacao
    {
        public int Id { get; set; }
        public int ContaId { get; set; } // Chave estrangeira

        public decimal Valor { get; set; }

        // Tipos: "Saque" ou "Deposito"
        public string Tipo { get; set; } = string.Empty;

        public DateTime Data { get; set; } = DateTime.Now;

        // Propriedade de navegação para o Entity Framework Core
        public Conta? Conta { get; set; }
    }
}