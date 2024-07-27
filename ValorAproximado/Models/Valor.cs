using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ValorAproximado.Models
{
    public class BodyEmpresa
    {
        public List<JsonElement> Data { get; set; }

        public BodyEmpresa()
        {
            Data = new List<JsonElement>();
        }
    }

}
