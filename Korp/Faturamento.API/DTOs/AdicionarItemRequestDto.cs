namespace Faturamento.API.DTOs {
    public sealed record AdicionarItemRequestDto {
        public Guid ProdutoId { get; init; }
        public required string CodigoProduto { get; init; }
        public required string DescricaoProduto { get; init; }
        public int Quantidade { get; init; }
    }
}
