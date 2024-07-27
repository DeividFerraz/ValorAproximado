using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System.Drawing;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;
using System.Xml.Linq;
using ValorAproximado.Data;
using ValorAproximado.Models;

namespace ValorAproximado.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ValorAproximadoController : ControllerBase
    {

        private readonly EmpresasGetDbContext _context;

        public ValorAproximadoController(EmpresasGetDbContext context)
        {
            this._context = context;
        }


        #region "Get Empresas"
        [HttpGet("TodasAsEmpresa")]
        public async Task<IActionResult> GetValor()
        {
            try
            {
                var empresa = await _context.Empresas.ToListAsync();
                return Ok(empresa);

            }
            catch (Exception ex)
            {
                return BadRequest($"Nenhuma empresa encontrada{ex.Message}");
            }
        }
        #endregion

        #region "Get Empresas nome"
        [HttpGet("PesquisaEmpresa")]
        public async Task<IActionResult> GetEmpresa([FromQuery] string nome)
        {
            try
            {
                var lista = from o in _context.Empresas.ToList()
                            where o.NomeEmpresa.ToUpper().Contains(nome.ToUpper())
                            select o;
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest($"Nenhuma empresa encontrada{ex.Message}");
            }
        }
        #endregion

        #region "Post Add Empresa"
        [HttpPost("AddEmpresa")]
        public async Task<IActionResult> PostEmpresas([FromBody] Empresas empresas)
        {
            try
            {
                await _context.Empresas.AddAsync(empresas);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok($"Sucesso{valor}");
                }
                else
                {
                    return BadRequest($"Nenhuma empresa incluida");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na inclusão da empresa {ex.Message}");
            }
        }
        #endregion

        #region "PostValor"
        [HttpPost("{valor}/{nomePropriedade}")]
        [SwaggerOperation(Summary = "Adicione um Json de Objetos no body, e no path coloque o nome da propriedade e o valor que deseja, para ver o objeto que chega mais próximo.", Description = "Este endpoint envia uma pergunta para o assistente GPT.")]
        public Task<IActionResult> PostValor([SwaggerParameter(Description = "Dados da pergunta")] 
        string nomePropriedade, decimal valor, [FromBody] List<BodyEmpresa> opcoes)
        {
            try
            {
                if (opcoes != null && opcoes.Count > 0)
                {
                    var closestMatch = opcoes.SelectMany(o => o.Data)
                        .Where(d => d.TryGetProperty(nomePropriedade, out JsonElement prop) && prop.ValueKind == JsonValueKind.Number)
                        .Select(d => new
                        {
                            Object = d,
                            Value = d.GetProperty(nomePropriedade).GetDecimal()
                        })
                        .OrderBy(d => Math.Abs(d.Value - valor))
                        .FirstOrDefault();

                    if (closestMatch != null)
                    {
                        return Task.FromResult<IActionResult>(Ok(closestMatch.Object));
                    }
                    else
                    {
                        return Task.FromResult<IActionResult>(NotFound("Nenhuma opção encontrada."));
                    }
                }
                else
                {
                    return Task.FromResult<IActionResult>(BadRequest("Nenhuma lista de opções fornecida no corpo da solicitação."));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult<IActionResult>(StatusCode(500, $"Ocorreu um erro: {ex.Message}"));
            }
        }
        #endregion
        #region "Explicação selectMany"
        /* Explicação do código: SelectMany = é usado para projetar cada elemento de uma sequência em uma coleção e achatar as coleções 
         * resultantes em uma única sequência. Neste caso, estamos pegando todas as propriedades Data de cada objeto em opcoes e criando uma única sequência.
        */
        #endregion

        #region "Explicação where"
        /*Where filtra a sequência de elementos com base em uma condição.Para cada elemento d(que é do tipo JsonElement), 
        verificamos se ele possui uma propriedade com o nome especificado por nomePropriedade e se essa propriedade é um número(JsonValueKind.Number).
        TryGetProperty tenta obter o valor da propriedade especificada e armazena-o em prop se bem-sucedido.*/
        #endregion

        #region "Explicação Select"
        /*Select projeta cada elemento da sequência em uma nova forma.
        Para cada d (elemento da sequência filtrada), criamos um objeto anônimo com duas propriedades: Object (o próprio elemento d) e 
        Value (o valor da propriedade nomePropriedade convertida para decimal).*/
        #endregion

        #region "Explicação OrderBy, que calcula o valor mais proximo do input"
        /*OrderBy classifica os elementos em ordem crescente com base na chave fornecida.
        A chave de ordenação é a diferença absoluta (Math.Abs) entre o valor da propriedade (d.Value) e o 
        valor especificado (valor). Isso é feito para encontrar o valor mais próximo de valor.*/
        #endregion

        #region"Explicação FirstOrDefault"
        /*FirstOrDefault retorna o primeiro elemento da sequência ordenada ou o valor padrão (nulo) se a sequência estiver vazia.
        O resultado final é o elemento da sequência cujo valor da propriedade nomePropriedade é o mais próximo de valor.*/
        #endregion

        #region "Resumo"
        /*Então, em resumo, este código está procurando no conjunto de dados (opcoes) o objeto cuja propriedade especificada (nomePropriedade) 
         * tem um valor numérico mais próximo de um valor fornecido (valor). Ele retorna o primeiro objeto que atende a este critério ou nulo se 
         * nenhum objeto for encontrado.*/
        #endregion
    }
}
