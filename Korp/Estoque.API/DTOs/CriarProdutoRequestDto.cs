namespace Estoque.API.DTOs {
    public sealed record CriarProdutoRequestDto(string Codigo, string Descricao, int Saldo);
}
