using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ValorAproximado.Models
{
    public class BodyEmpresa
    {
        public int Index { get; set; }
        public string CodigoOpcao { get; set; }
        public int QtdParcelas { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public bool AVista { get; set; }
        public decimal ValorEntrada { get; set; }
        public decimal ValorTotal { get; set; }
        public string DataVencimento { get; set; }
        public string ValorDesconto { get; set; }
        public decimal ValorTotal1 { get; set; }
        public int PercentualJuros { get; set; }
    }
}
