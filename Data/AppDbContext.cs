using Microsoft.EntityFrameworkCore;
using Api.Models; // Certifique-se de que sua pasta de modelos se chama Banco.Models

namespace Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // Representa a tabela de Contas (Corrente, Poupança, etc)
        public DbSet<Conta> Contas { get; set; }

        // Representa a tabela de Transações (Saques e Depósitos)
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
