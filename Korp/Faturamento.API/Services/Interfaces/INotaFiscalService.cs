using Faturamento.API.DTOs;

namespace Faturamento.API.Services.Interfaces {
    public interface INotaFiscalService {
        Task<IReadOnlyList<NotaFiscalResponseDto>> ListarAsync(CancellationToken cancellationToken);

        Task<NotaFiscalResponseDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

        Task<NotaFiscalResponseDto> CriarAsync(CancellationToken cancellationToken);

        Task<NotaFiscalResponseDto> AdicionarItemAsync(Guid notaId, AdicionarItemRequestDto request, CancellationToken cancellationToken);

        Task<NotaFiscalResponseDto> RemoverItemAsync(Guid notaId, Guid itemId, CancellationToken cancellationToken);

        Task<NotaFiscalResponseDto> ImprimirAsync(Guid notaId, CancellationToken cancellationToken);
    }
}
