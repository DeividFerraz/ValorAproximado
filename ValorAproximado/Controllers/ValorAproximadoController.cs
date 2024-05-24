using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
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

            }catch (Exception ex)
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

            }catch (Exception ex)
            {
                return BadRequest($"Erro na inclusão da empresa {ex.Message}");
            }
        }
        #endregion

        #region "PostValor"
        [HttpPost("{valor}")]
        public Task<IActionResult> PostValor(decimal valor, [FromBody] List<BodyEmpresa> opcoes)
        {
            try
            {
                if (opcoes != null && opcoes.Count > 0)
                {
                    /*opcoes.OrderBy(o => Math.Abs(o.ValorEntrada - valor)): Aqui, a coleção opcoes é 
                      ordenada com base em uma expressão lambda que calcula a diferença absoluta entre a 
                      propriedade ValorEntrada de cada objeto (o.ValorEntrada) e o valor valor. 
                      A função OrderBy() ordena os elementos em ordem crescente com base nessa diferença absoluta.
                 FirstOrDefault(): Esta função retorna o primeiro elemento da coleção ordenada. 
                 Como a coleção foi ordenada com base na diferença absoluta, o primeiro elemento será aquele 
                 com a menor diferença absoluta em relação ao valor fornecido valor. Se a coleção estiver vazia,
                 FirstOrDefault() retornará o valor padrão para o tipo de objeto (que é null para tipos de 
                 referência).*/
                    var valorAproximado = opcoes.OrderBy(o => Math.Abs(o.ValorEntrada - valor)).FirstOrDefault();

                    if (valorAproximado != null)
                    {
                        return Task.FromResult<IActionResult>(Ok(valorAproximado));
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

        #region "PostValor"
        [HttpPost("/API/Paschoalotto{valor}")]
        public Task<IActionResult> PostValor1(decimal valor, [FromBody] List<BodyEmpresa> opcoes)
        {
            try
            {
                if (opcoes != null && opcoes.Count > 0)
                {
                    var valorAproximado = opcoes.OrderBy(o => Math.Abs(o.ValorTotal - valor)).FirstOrDefault();

                    if (valorAproximado != null)
                    {
                        return Task.FromResult<IActionResult>(Ok(valorAproximado));
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
    }
}
