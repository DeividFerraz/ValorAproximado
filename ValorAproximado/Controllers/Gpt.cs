using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ValorAproximado.Data;
using ValorAproximado.Models;

namespace ValorAproximado.Controllers
{

    [ApiController]
    [Route("chatGpt")]
    public class GptController : Controller
    {
        private readonly HttpClient _context;

        public GptController(HttpClient context)
        {
            _context = context;
        }

        [HttpPost] 
        public async Task<IActionResult> PostGPT([FromBody] string text, [FromServices] IConfiguration configuration)
        {
            var token = configuration.GetValue<string>("ChaveGPT");

            _context.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var gptRequest = new GptRequest(text);

            var requestBody = JsonSerializer.Serialize(gptRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var response = await _context.PostAsync("https://api.openai.com/v1/chat/completions", content);

            var result = await response.Content.ReadFromJsonAsync<GptRetorno>();

            if (result != null && result.choices != null && result.choices.Count > 0)
            {
                var valorRetornado = result.choices[0];
                return Ok(valorRetornado);
            }
            else
            {
                return NotFound("Não foram encontradas choices na resposta.");
            }
            
        }

    }
}
