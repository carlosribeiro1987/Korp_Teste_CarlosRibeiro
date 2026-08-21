using Faturamento.API.Models;

namespace Faturamento.API.DTOs {
    public sealed record ItemNotaFiscalResponseDto{
        public Guid ItemId { get; init; }
        public Guid ProdutoId { get; init; }
        public string CodigoProduto { get; init; } = string.Empty;
        public string DescricaoProduto { get; init; } = string.Empty;
        public int Quantidade { get; init; }

        public static ItemNotaFiscalResponseDto FromModel(ItemNotaFiscal item) {
            return new ItemNotaFiscalResponseDto {
                ItemId = item.ItemId,
                ProdutoId = item.ProdutoId,
                CodigoProduto = item.CodigoProduto,
                DescricaoProduto = item.DescricaoProduto,
                Quantidade = item.Quantidade
            };
        }
    }
}
