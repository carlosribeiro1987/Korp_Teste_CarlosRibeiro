namespace Estoque.API.DTOs {
    public sealed record CriarProdutoRequestDto{
        public required string Codigo { get; init; }
        public required string Descricao { get; init; }
        public int Saldo { get; init; }
    }
}
