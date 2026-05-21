namespace Api.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty; // Adicione esta linha se não existir!
        public string Cpf { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty; // Vamos manter em texto puro para poupar seu tempo!

        // Relação: Um usuário pode ter uma conta (ou várias, mas faremos 1 para 1 para simplificar)
        public Conta? Conta { get; set; }
    }
}
