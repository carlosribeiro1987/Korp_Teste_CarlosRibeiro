using Estoque.API.DTOs;

namespace Estoque.API.Services.Interfaces {
    public interface IProdutoService {
        Task<IReadOnlyList<ProdutoResponseDto>> ListarAsync(CancellationToken cancellationToken);
        Task<ProdutoResponseDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
        Task<ProdutoResponseDto> CriarAsync(CriarProdutoRequestDto request, CancellationToken cancellationToken);
        Task<ProdutoResponseDto> ReduzirSaldoAsync(Guid id, int quantidade, CancellationToken cancellationToken);
        Task<ProdutoResponseDto> AumentarSaldoAsync(Guid id, int quantidade, CancellationToken cancellationToken);
    }
}
