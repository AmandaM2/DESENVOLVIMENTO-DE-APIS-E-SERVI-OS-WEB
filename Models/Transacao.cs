using System;

namespace Api.Models
{
    public class Transacao
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataHora { get; set; } = DateTime.Now;
        public TipoTransacao Tipo { get; set; }
        public int ContaId { get; set; } // Chave estrangeira

        // Propriedade de navegação para o Entity Framework Core
        public Conta? Conta { get; set; }
    }
}
