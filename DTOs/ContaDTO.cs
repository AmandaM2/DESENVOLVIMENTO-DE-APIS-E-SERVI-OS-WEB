using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    public class ContaDTO
    {
        [Required(ErrorMessage = "O nome do titular é obrigatório")]
        public string Titular { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de conta é obrigatório")]
        // Aceita: Corrente, Poupança ou Empresarial
        public string TipoConta { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "O saldo inicial não pode ser negativo")]
        public decimal SaldoInicial { get; set; }
    }
}
