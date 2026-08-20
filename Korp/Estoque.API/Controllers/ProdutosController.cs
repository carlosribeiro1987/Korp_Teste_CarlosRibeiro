using Estoque.API.DTOs;
using Estoque.API.Exceptions;
using Estoque.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.API.Controllers {
    [Route("api/produtos")]
    [ApiController]
    public sealed class ProdutosController : ControllerBase {
        private readonly IProdutoService _service;

        public ProdutosController(IProdutoService service) {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProdutoResponseDto>>> ListarProdutos(CancellationToken cancellationToken) {
            var result = await _service.ListarAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoResponseDto>> ObterProduto(Guid id, CancellationToken cancellationToken) {
            var produto = await _service.ObterPorIdAsync(id, cancellationToken);
            return produto is null ? NotFound() : Ok(produto);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoResponseDto>> CriarProduto(CriarProdutoRequestDto request, CancellationToken cancellationToken) {
            try {
                var produto = await _service.CriarAsync(request, cancellationToken);
                return CreatedAtAction(nameof(ObterProduto), new { id = produto.Id }, produto);
            }
            catch(ArgumentException ex) {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch(InvalidOperationException ex) {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        [HttpPost("{id:guid}/reduzir-saldo")]
        public async Task<ActionResult<ProdutoResponseDto>> ReduzirSaldo(Guid id, ReduzirSaldoRequestDto request, CancellationToken cancellationToken) {
            try {
                var produto = await _service.ReduzirSaldoAsync(id, request.Quantidade, cancellationToken);
                return Ok(produto);
            }
            catch(EstoqueIndisponivelException ex) {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { mensagem = ex.Message });
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
