using Faturamento.API.Models;

namespace Faturamento.API.DTOs {
    public sealed record NotaFiscalResponseDto {
        public Guid Id { get; init; }
        public int Numero { get; init; }
        public string Status { get; init; } = string.Empty;
        public DateTime CriadaEm { get; init; }
        public DateTime? FechadaEm { get; init; }
        public IReadOnlyCollection<ItemNotaFiscalResponseDto> Itens { get; init; } = [];


        public static NotaFiscalResponseDto FromModel(NotaFiscal nota) {
            return new NotaFiscalResponseDto {
                Id = nota.NotaFiscalId,
                Numero = nota.Numero,
                Status = nota.Status.ToString(),
                CriadaEm = nota.CriadaEm,
                FechadaEm = nota.FechadaEm,
                Itens = nota.Itens.Select(ItemNotaFiscalResponseDto.FromModel).ToList()
            };
        }
    }
}
