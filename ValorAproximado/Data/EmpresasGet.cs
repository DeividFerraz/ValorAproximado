using Microsoft.EntityFrameworkCore;
using ValorAproximado.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ValorAproximado.Data
{
    public class EmpresasGetDbContext : DbContext
    {
        //É obrigatório essa estrutura e padrão
        public EmpresasGetDbContext(DbContextOptions<EmpresasGetDbContext> options) : base(options)
        {

        }

        public DbSet<Empresas> Empresas { get; set; }
        public DbSet<ConversationMessage> ConversationMessage { get; set; }
        public DbSet<ConversationRequest> ConversationRequest { get; set; }
    }
}
