using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 20 caracteres")]
        public string Senha { get; set; } = string.Empty;
    }
}
