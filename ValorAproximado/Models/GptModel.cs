namespace ValorAproximado.Models
{
    public class GetModel
    {
        public string Role { get; set; }
        public string Content { get; set; }

        // Construtor para facilitar a criação de mensagens
        public GetModel(string role, string content)
        {
            Role = role;
            Content = content;
        }
    }

    public class GptRequest
    {
        public GptRequest(string initialContent)
        {
            Model = "gpt-3.5-turbo-1106";
            Messages = new List<GetModel>
            {
                // Supondo que você sempre quer iniciar com uma mensagem de sistema
                new GetModel("system", $"Você é uma assistente útil. {initialContent}")
            };
        }

        public string Model { get; set; }
        public List<GetModel> Messages { get; set; }

        // Método para adicionar mensagens adicionais, se necessário
        public void AddMessage(string role, string content)
        {
            Messages.Add(new GetModel(role, content));
        }
    }
}
