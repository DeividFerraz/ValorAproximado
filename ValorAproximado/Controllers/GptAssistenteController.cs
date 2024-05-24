using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ValorAproximado.Models;

namespace ValorAproximado.Controllers
{
    [ApiController]
    [Route("gptAssistente")]
    public class GptAssistenteController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public GptAssistenteController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost]
        public async Task<ActionResult> PostMessage([FromBody] string text, [FromServices] IConfiguration configuration)
        {
            var token = configuration.GetValue<string>("ChaveGPT");

            var gptRequest = new GptAssistente("user", text);

            var requestBody = JsonSerializer.Serialize(gptRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/threads/thread_olPpqVLTTvOHyfH20eWAC4bB/messages", content);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Ok(responseBody);
            }
            else
            {
                return StatusCode((int)response.StatusCode, $"Erro ao chamar a API do OpenAI: {responseBody}");
            }
        }

        [HttpPost("execucao")]
        public async Task<ActionResult> PostExecucao([FromQuery] string threadId, [FromBody] ExecucaoRequest request, [FromServices] IConfiguration configuration)
        {
            var token = configuration.GetValue<string>("ChaveGPT");

            var gptRequest = new GptExecucao(request.AssistantId);

            var requestBody = JsonSerializer.Serialize(gptRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

            var response = await _httpClient.PostAsync($"https://api.openai.com/v1/threads/{threadId}/runs", content);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<GptExecucaoRetorno>(responseBody);
                return Ok(result);
            }
            else
            {
                return StatusCode((int)response.StatusCode, $"Erro ao chamar a API do OpenAI: {responseBody}");
            }
        }

        [HttpGet("mensagens")]
        public async Task<ActionResult> GetMensagens([FromQuery] string threadId, [FromServices] IConfiguration configuration)
        {
            var token = configuration.GetValue<string>("ChaveGPT");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/threads/{threadId}/messages");

            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<GptListaMensagem>(responseBody);
                return Ok(result);
            }
            else
            {
                return StatusCode((int)response.StatusCode, $"Erro ao chamar a API do OpenAI: {responseBody}");
            }
        }

        [HttpPost("pergunta")]
        public async Task<ActionResult> Pergunta(
    [FromBody] PerguntaRequest request,
    [FromHeader(Name = "Authorization")] string authorizationHeader,
    [FromHeader(Name = "Thread_Id")] string tread,
    [FromHeader(Name = "Assistant_Id")] string assistent,
    [FromServices] IConfiguration configuration)
        {
            var token = authorizationHeader.Replace("Bearer ", "");
            var threadId = tread;
            var assistantId = assistent;

            var mensagemRequest = new GptAssistenteMensagem("user", request.Pergunta);
            var mensagemRequestBody = JsonSerializer.Serialize(mensagemRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var mensagemContent = new StringContent(mensagemRequestBody, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

            var mensagemResponse = await _httpClient.PostAsync($"https://api.openai.com/v1/threads/{threadId}/messages", mensagemContent);
            if (!mensagemResponse.IsSuccessStatusCode)
            {
                var errorContent = await mensagemResponse.Content.ReadAsStringAsync();
                return StatusCode((int)mensagemResponse.StatusCode, new { message = $"Erro ao criar a mensagem: {errorContent}" });
            }

            var gptExecucaoRequest = new GptExecucao(assistantId);
            var execucaoRequestBody = JsonSerializer.Serialize(gptExecucaoRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var execucaoContent = new StringContent(execucaoRequestBody, Encoding.UTF8, "application/json");

            var execucaoResponse = await _httpClient.PostAsync($"https://api.openai.com/v1/threads/{threadId}/runs", execucaoContent);
            if (!execucaoResponse.IsSuccessStatusCode)
            {
                var errorContent = await execucaoResponse.Content.ReadAsStringAsync();
                return StatusCode((int)execucaoResponse.StatusCode, new { message = $"Erro ao criar a execução: {errorContent}" });
            }

            await Task.Delay(3000);

            var mensagensResponse = await _httpClient.GetAsync($"https://api.openai.com/v1/threads/{threadId}/messages");
            var mensagensResponseBody = await mensagensResponse.Content.ReadAsStringAsync();

            if (!mensagensResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)mensagensResponse.StatusCode, new { message = $"Erro ao obter as mensagens: {mensagensResponseBody}" });
            }

            var gptListaMensagem = JsonSerializer.Deserialize<GptListaMensagem>(mensagensResponseBody);

            var respostaAssistente = gptListaMensagem?.Data?.Find(m => m.Role == "assistant");
            if (respostaAssistente != null)
            {
                return Ok(new { message = respostaAssistente.Content[0].Text.Value });
            }

            return NotFound(new { message = "Não foi possível encontrar a resposta do assistente." });
        }
    }
}