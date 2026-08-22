namespace Estoque.API.DTOs {
    public sealed record ReduzirSaldoRequestDto {
        public int Quantidade { get; init; }
    }
}
