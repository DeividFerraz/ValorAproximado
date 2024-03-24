using System.ComponentModel.DataAnnotations;

namespace ValorAproximado.Models
{
    public class Empresas
    {
       [Key]
       public Guid id { get; set; }
       public string NomeEmpresa { get; set; }
    }
}
