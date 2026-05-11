namespace Api.DTOs
{
    // Olha como ele é limpo! Só tem o que o usuário DEVE preencher.
    public class ContaCreateDTO
    {
        public string Titular { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
    }
}
