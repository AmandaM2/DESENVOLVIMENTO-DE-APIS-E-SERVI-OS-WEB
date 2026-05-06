using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class TransacaoDTO
    {
        [Required]
        public int ContaId { get; set; }

        [Required(ErrorMessage = "O valor da operação é obrigatório")]
        [Range(0.01, 10000, ErrorMessage = "O valor deve ser entre 0.01 e 10.000")]
        public decimal Valor { get; set; }

        [Required]
        // Define se é "Saque" ou "Deposito"
        public string TipoOperacao { get; set; } = string.Empty;
    }
}