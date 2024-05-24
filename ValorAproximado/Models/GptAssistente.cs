using System.Text.Json.Serialization;

namespace ValorAproximado.Models
{
    public class GptAssistente
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        public GptAssistente(string role, string content)
        {
            Role = role;
            Content = content;
        }
    }

    public class GptExecucao
    {
        [JsonPropertyName("assistant_id")]
        public string AssistantId { get; set; }

        public GptExecucao(string assistantId)
        {
            AssistantId = assistantId;
        }
    }
    public class ExecucaoRequest
    {
        public string AssistantId { get; set; }
    }
    public class PerguntaRequest
    {
        [JsonPropertyName("pergunta")]
        public string Pergunta { get; set; }
    }
    public class GptAssistenteMensagem
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        public GptAssistenteMensagem(string role, string content)
        {
            Role = role;
            Content = content;
        }
    }

}

