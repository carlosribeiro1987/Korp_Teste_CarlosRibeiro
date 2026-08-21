using Faturamento.API.Clients.Interfaces;
using Faturamento.API.Exceptions;
using System.Net;

namespace Faturamento.API.Clients {
    public sealed class EstoqueClient : IEstoqueClient {
        private readonly HttpClient _httpClient;

        public EstoqueClient(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        public async Task ReduzirSaldoAsync(Guid produtoId, int quantidade, CancellationToken cancellationToken) {
            HttpResponseMessage response;
            try {
                response = await _httpClient.PostAsJsonAsync($"api/produtos/{produtoId}/reduzir-saldo", 
                                                             new { Quantidade = quantidade }, cancellationToken);
            }
            catch(HttpRequestException ex) {
                throw new EstoqueIndisponivelException("Nao foi possivel conectar ao servico de estoque.", ex);
            }
            catch(TaskCanceledException ex) when(!cancellationToken.IsCancellationRequested) {
                throw new EstoqueIndisponivelException("Tempo esgotado ao comunicar com o servico de estoque.", ex);
            }

            if(response.StatusCode == HttpStatusCode.ServiceUnavailable) {
                throw new EstoqueIndisponivelException("O servico de estoque esta indisponivel.");
            }

            if(!response.IsSuccessStatusCode) {
                var detalhe = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new EstoqueApiException($"Falha ao reduzir saldo. Status {(int)response.StatusCode}. {detalhe}");
            }
        }
    }
}
