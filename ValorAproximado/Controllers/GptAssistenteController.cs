using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ValorAproximado.Models;
using static System.Net.WebRequestMethods;

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
        [SwaggerOperation(Summary = "Adicione os ids de thread e assistente, depois, faça sua pergunta", Description = "Este endpoint envia uma pergunta para o assistente GPT.")]
        public async Task<ActionResult> Pergunta(
             [SwaggerParameter(Description = "Dados da pergunta")]
    [FromBody] PerguntaRequest request,
    [FromHeader(Name = "Authorization")] string authorizationHeader,
    [FromHeader(Name = "threadId")] string tread,
    [FromHeader(Name = "assistantId")] string assistent,
    [FromServices] IConfiguration configuration)
        {
            var token = authorizationHeader.Replace("Bearer ", "");
            var threadId = tread;
            var assistantId = assistent;

            var mensagemRequest = new GptAssistenteMensagem("user", request.Pergunta);
            var mensagemRequestBody = JsonSerializer.Serialize(mensagemRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            //Esta linha serializa (converte para JSON) o objeto mensagemRequest.                       define que as propriedades do objeto no JSON resultante devem
            //A serialização é feita usando a classe JsonSerializer                                    //seguir a convenção camelCase (por exemplo, perguntaDoUsuario em vez de
                                                                                                                                                    //PerguntaDoUsuario).

            var mensagemContent = new StringContent(mensagemRequestBody, Encoding.UTF8, "application/json");//O primeiro argumento para o construtor de StringContent é a string
                                                                                                            //que você deseja enviar no corpo da solicitação HTTP. Neste caso,
                                                                                                            //é o JSON representado pela variável mensagemRequestBody
                                                                                                            //
                                                                                                            //Encoding.UTF8 indica que a string deve ser codificada usando UTF-8,
                                                                                                            //uma codificação padrão para intercâmbio de dados na web.
                                                                                                            //
                                                                                                            //StringContent é uma classe no namespace System.Net.Http usada
                                                                                                            //para representar o conteúdo HTTP como uma string.
                                                                                                            //É usada para enviar dados em requisições HTTP POST ou PUT
                                                                                                            //
                                                                                                            //"application/json" indica que o conteúdo é um JSON, informando ao
                                                                                                            //servidor que ele deve interpretar o corpo da solicitação como um
                                                                                                            //objeto JSON.


            _httpClient.DefaultRequestHeaders.Clear();/*limpa todoa os cabeçalhos que podem ter sido configurado anteriormente*/
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);/* Aqui, você está adicionando um cabeçalho de autorização ao HttpClient*/
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");/*Esta linha adiciona um cabeçalho personalizado chamado "OpenAI-Beta" ao HttpClient. 
                                                                                   * O valor desse cabeçalho é "assistants=v2", que provavelmente está indicando que a 
                                                                                   * solicitação deve usar uma versão beta específica ou funcionalidade experimental 
                                                                                   * dos assistentes da OpenAI.*/

            var mensagemResponse = await _httpClient.PostAsync($"https://api.openai.com/v1/threads/{threadId}/messages", mensagemContent);
            if (!mensagemResponse.IsSuccessStatusCode)
            {
                var errorContent = await mensagemResponse.Content.ReadAsStringAsync();//lê o conteúdo da resposta de erro como uma string assíncronamente.
                return StatusCode((int)mensagemResponse.StatusCode, new { message = $"Erro ao criar a mensagem: {errorContent}" });
                //StatusCode((int)mensagemResponse.StatusCode, new { message = $"Erro ao criar a mensagem: {errorContent}" });
                //converte o código de status para um inteiro e cria um objeto anônimo com uma propriedade "message" que contém a mensagem de erro.
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
            var mensagensResponseBody = await mensagensResponse.Content.ReadAsStringAsync();//lê o conteúdo da resposta como uma string assíncronamente.

            if (!mensagensResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)mensagensResponse.StatusCode, new { message = $"Erro ao obter as mensagens: {mensagensResponseBody}" });
            }

            var gptListaMensagem = JsonSerializer.Deserialize<GptListaMensagem>(mensagensResponseBody);/*desserializa uma string JSON (mensagensResponseBody) 
            * em uma instância do tipo GptListaMensagem. Isso permite que você trabalhe com os dados JSON como objetos C# fortemente tipados, facilitando o acesso 
            * e a manipulação dos dados.*/

            /*Em suma, eu vou pegar o resultado de mensagensResponseBody que é um Objeto Json, e vou montar ele usando os atributos da classe GptListaMensagem.
             * Para que isso de certo, os nomes dos atributos da classe GptListaMensagem precisam ser igual aos da resposta retornada em mensagensResponseBody
             para que essa Deserialização de certo.*/


            /*Resumindo, essa linha de código procura a primeira mensagem na lista gptListaMensagem.Data cuja propriedade Role seja igual a "assistant". 
             * Se encontrar, essa mensagem será armazenada na variável respostaAssistente; caso contrário, respostaAssistente será null.*/
            var respostaAssistente = gptListaMensagem?.Data?.Find(m => m.Role == "assistant");
            //Estou dizendo que a primeira lista de Data que é representado por "m" se nessa lista tiver um Role representado por "m.Role"
            //Eu estou dizendo que quando alguma dessas listas dentro de data, tiver um Role que é m => m.Role, me traga esse objeto, Usando
            //Find eu quis dizer que o primeiro que acharem me traga, e usando o "== assistant" quis dizer que o primeiro que achar e sendo igual
            //assistante me tragam esse objeto.
            if (respostaAssistente != null)
            {
                return Ok(new { message = respostaAssistente.Content[0].Text.Value });
            }

            return NotFound(new { message = "Não foi possível encontrar a resposta do assistente." });
        }
    }
}