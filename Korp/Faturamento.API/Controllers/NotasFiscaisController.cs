using Faturamento.API.DTOs;
using Faturamento.API.Exceptions;
using Faturamento.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Faturamento.API.Controllers {
    [Route("api/notas-fiscais")]
    [ApiController]
    public sealed class NotasFiscaisController : ControllerBase {
        private readonly INotaFiscalService _service;

        public NotasFiscaisController(INotaFiscalService service) {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Listar notas fiscais")]
        public async Task<ActionResult<IReadOnlyList<NotaFiscalResponseDto>>> Listar(CancellationToken cancellationToken) {
            return Ok(await _service.ListarAsync(cancellationToken));
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Obter nota fiscal por ID")]
        public async Task<ActionResult<NotaFiscalResponseDto>> Obter(Guid id, CancellationToken cancellationToken) {
            var nota = await _service.ObterPorIdAsync(id, cancellationToken);
            return nota is null ? NotFound() : Ok(nota);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Criar nota fiscal")]
        public async Task<ActionResult<NotaFiscalResponseDto>> Criar(CancellationToken cancellationToken) {
            var nota = await _service.CriarAsync(cancellationToken);
            return CreatedAtAction(nameof(Obter), new { id = nota.Id }, nota);
        }

        [HttpPost("{id:guid}/itens")]
        [SwaggerOperation(Summary = "Adicionar itens a uma nota fiscal")]
        public async Task<ActionResult<NotaFiscalResponseDto>> AdicionarItem(Guid id, AdicionarItemRequestDto request,
                                                                             CancellationToken cancellationToken) {
            try {
                return Ok(await _service.AdicionarItemAsync(id, request, cancellationToken));
            }
            catch(KeyNotFoundException ex) {
                return NotFound(new { mensagem = ex.Message });
            }
            catch(InvalidOperationException ex) {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpDelete("{id:guid}/itens/{itemId:guid}")]
        [SwaggerOperation(Summary = "Remover itens de uma nota fiscal")]
        public async Task<ActionResult<NotaFiscalResponseDto>> RemoverItem(Guid id, Guid itemId, 
                                                                           CancellationToken cancellationToken) {
            try {
                return Ok(await _service.RemoverItemAsync(id, itemId, cancellationToken));
            }
            catch(KeyNotFoundException ex) {
                return NotFound(new { mensagem = ex.Message });
            }
            catch(InvalidOperationException ex) {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost("{id:guid}/imprimir")]
        [SwaggerOperation(Summary = "Imprimir uma nota fiscal")]
        public async Task<ActionResult<NotaFiscalResponseDto>> Imprimir(Guid id, CancellationToken cancellationToken) {
            try {
                return Ok(await _service.ImprimirAsync(id, cancellationToken));
            }
            catch(EstoqueIndisponivelException ex) {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { mensagem = ex.Message });
            }
            catch(EstoqueApiException ex) {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch(KeyNotFoundException ex) {
                return NotFound(new { mensagem = ex.Message });
            }
            catch(InvalidOperationException ex) {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}
