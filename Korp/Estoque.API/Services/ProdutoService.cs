using Estoque.API.Data;
using Estoque.API.DTOs;
using Estoque.API.Models;
using Estoque.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Estoque.API.Services {
    public sealed class ProdutoService : IProdutoService {
        private readonly EstoqueDbContext _dbContext;


        public ProdutoService(EstoqueDbContext dbContext) {
            _dbContext = dbContext;
        }

        

        public async Task<ProdutoResponseDto> CriarAsync(CriarProdutoRequestDto request, CancellationToken cancellationToken) {
            var codigoExiste = await _dbContext.Produtos.AnyAsync(p => p.Codigo == request.Codigo, cancellationToken);
            if(codigoExiste) {
                throw new InvalidOperationException("Já existe um produto com esse código.");
            }
            var produto = new Produto(request.Codigo, request.Descricao, request.Saldo);
            _dbContext.Add(produto);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ProdutoResponseDto.FromModel(produto);
        }

        public async Task<IReadOnlyList<ProdutoResponseDto>> ListarAsync(CancellationToken cancellationToken) {
            var result = await _dbContext.Produtos
                                .AsNoTracking()
                                .OrderBy(produto => produto.Codigo)
                                .Select(produto => ProdutoResponseDto.FromModel(produto))
                                .ToListAsync(cancellationToken);
            return result;
        }

        public async Task<ProdutoResponseDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) {
            var produto = await _dbContext.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            return produto is null ? null : ProdutoResponseDto.FromModel(produto);
        }

        public async Task<ProdutoResponseDto> ReduzirSaldoAsync(Guid id, int quantidade, CancellationToken cancellationToken) {
            var produto = await _dbContext.Produtos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if(produto is null) {
                throw new KeyNotFoundException("Produto não encontrado.");
            }
            produto.ReduzirSaldo(quantidade);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ProdutoResponseDto.FromModel(produto);
        }

        public async Task<ProdutoResponseDto> AumentarSaldoAsync(Guid id, int quantidade, CancellationToken cancellationToken) {
            var produto = await _dbContext.Produtos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if(produto is null) {
                throw new KeyNotFoundException("Produto não encontrado.");
            }
            produto.AumentarSaldo(quantidade);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ProdutoResponseDto.FromModel(produto);
        }
    }
    
}
