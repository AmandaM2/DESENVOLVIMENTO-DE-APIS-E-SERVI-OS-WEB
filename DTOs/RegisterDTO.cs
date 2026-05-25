using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string? Usuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string? Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter exatamente 11 números.")]
        public string? Cpf { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
        public string? Senha { get; set; } = string.Empty;

        [Required]
        [RegularExpression("Corrente|Poupança", ErrorMessage = "Tipo de conta inválido.")]
        public string? TipoConta { get; set; } = string.Empty;
    }
}
