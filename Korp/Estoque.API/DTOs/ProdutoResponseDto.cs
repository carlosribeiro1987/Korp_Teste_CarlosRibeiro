using Estoque.API.Models;

namespace Estoque.API.DTOs {
    public sealed record ProdutoResponseDto(Guid Id, string Codigo, string Descricao, int saldo, DateTime CriadoEm, DateTime ModificadoEm) {
        public static ProdutoResponseDto FromModel(Produto produto) {
            return new ProdutoResponseDto(produto.Id, produto.Codigo, produto.Descricao, produto.Saldo, produto.CriadoEm, produto.ModificadoEm);
        }
    }
}
