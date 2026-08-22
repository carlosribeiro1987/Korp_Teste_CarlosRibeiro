using Estoque.API.Models;

namespace Estoque.API.DTOs {
    public sealed record ProdutoResponseDto {

        public Guid Id { get; init; }
        public string Codigo { get; init; } = string.Empty;
        public string Descricao { get; init; } = string.Empty;
        public int Saldo { get; init; }
        public DateTimeOffset CriadoEm { get; init; }
        public DateTimeOffset ModificadoEm { get; init; }

        public static ProdutoResponseDto FromModel(Produto produto) {
            return new ProdutoResponseDto {
                Id = produto.Id,
                Codigo = produto.Codigo,
                Descricao = produto.Descricao,
                Saldo = produto.Saldo,
                CriadoEm = produto.CriadoEm,
                ModificadoEm = produto.ModificadoEm
            };
        }
    }
}
