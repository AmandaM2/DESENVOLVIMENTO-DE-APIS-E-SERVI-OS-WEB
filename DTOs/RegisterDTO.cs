using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter exatamente 11 números.")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de conta é obrigatório.")]
        [RegularExpression("Corrente|Poupança", ErrorMessage = "Tipo de conta inválido. Escolha 'Corrente' ou 'Poupança'.")]
        public string TipoConta { get; set; } = string.Empty;

        // --- ADICIONE ESTA LINHA PARA CASAR COM O SCRIPT.JS ---
        [Required]
        public bool AceitouLgpd { get; set; }
    }
}
