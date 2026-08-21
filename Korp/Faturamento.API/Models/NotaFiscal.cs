using Faturamento.API.Enums;

namespace Faturamento.API.Models {
    public sealed class NotaFiscal {
        private readonly List<ItemNotaFiscal> _itens = [];

        public Guid NotaFiscalId { get; private set; }
        public int Numero { get; private set; }
        public StatusNotaFiscal Status { get; private set; }
        public DateTime CriadaEm { get; private set; }
        public DateTime? FechadaEm { get; private set; }
        public IReadOnlyCollection<ItemNotaFiscal> Itens => _itens;

        private NotaFiscal() { }

        public NotaFiscal(int numero) {
            NotaFiscalId = Guid.NewGuid();
            Numero = numero;
            Status = StatusNotaFiscal.Aberta;
            CriadaEm = DateTime.UtcNow;
        }

        public void AdicionarItem(Guid produtoId, string codigoProduto, string descricaoProduto, int quantidade) {
            if(Status is not StatusNotaFiscal.Aberta) {
                throw new InvalidOperationException("Apenas notas abertas podem ser alteradas.");
            }
            if(quantidade <= 0) {
                throw new InvalidOperationException("Quantidade deve ser maior que zero.");
            }
            var item = new ItemNotaFiscal(NotaFiscalId, produtoId, codigoProduto, descricaoProduto, quantidade);

            _itens.Add(item);
        }

        public void RemoverItem(Guid itemId) {
            if(Status is not StatusNotaFiscal.Aberta) {
                throw new InvalidOperationException("Apenas notas abertas podem ser alteradas.");
            }
            var item = _itens.FirstOrDefault(i => i.ItemId == itemId);

            if(item is null) {
                throw new InvalidOperationException("Item nao encontrado na nota fiscal.");
            }            

            _itens.Remove(item);
        }

        public void Fechar() {
            if(Status != StatusNotaFiscal.Aberta) {
                throw new InvalidOperationException("Apenas notas abertas podem ser fechadas.");
            }
            if(_itens.Count == 0) {
                throw new InvalidOperationException("Nota fiscal precisa ter pelo menos um item.");
            }

            Status = StatusNotaFiscal.Fechada;
            FechadaEm = DateTime.UtcNow;
        }
    }
}
