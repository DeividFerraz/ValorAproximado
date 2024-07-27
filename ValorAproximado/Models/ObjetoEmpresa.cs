namespace ValorAproximado.Models
{
    public class ObjetoEmpresa
    {
        public List<string> Strings { get; set; }
        public string NomePropriedade { get; set; }

        public ObjetoEmpresa(string nomePropriedade)
        {
            NomePropriedade = nomePropriedade;
            Strings = new List<string>();
        }
    }
}
