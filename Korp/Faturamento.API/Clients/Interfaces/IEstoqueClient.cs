namespace Faturamento.API.Clients.Interfaces {
    public interface IEstoqueClient {
        Task ReduzirSaldoAsync(Guid produtoId, int quantidade, CancellationToken cancellationToken);
    }
}
